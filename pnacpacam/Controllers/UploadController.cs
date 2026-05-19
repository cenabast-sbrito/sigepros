using ClosedXML.Excel;
using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace pnacpacam.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class UploadController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1️⃣ Sesión expirada
            if (Session == null || Session.IsNewSession)
            {
                CerrarSesion(filterContext);
                return;
            }

            // 2️⃣ Forms Authentication inválido
            if (!Request.IsAuthenticated || User == null || !User.Identity.IsAuthenticated)
            {
                CerrarSesion(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private void CerrarSesion(ActionExecutingContext filterContext)
        {
            // Limpiar sesión
            Session.Clear();
            Session.Abandon();

            // Cerrar Forms Authentication
            FormsAuthentication.SignOut();

            // Eliminar cookie
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            // Redirigir a Login (acciones que devuelven View)
            filterContext.Result = RedirectToAction("Index", "Login");
        }
        public JsonResult getZDis022(string topx)
        {

            PlanillaZDIS022 despacho = new PlanillaZDIS022();

            var despachos = despacho.getZDis022(topx, "Despachos_TEMP");

            return Json(despachos, JsonRequestBehavior.AllowGet);

        }
        /// 
        /// /* carga planilla zdis022  */
        /// 
        public JsonResult UploadFile(HttpPostedFileBase archivoExcel, string fechaCarga, string rutProveedorCarga, string programaDefinido, bool chkFiltroPeriodo)
        { //fechaCarga llega en el formato 2024-07
            var request = HttpContext.Request;
            PlanillaZDIS022 despacho;
            try
            {
                using (var workbook = new XLWorkbook(archivoExcel.InputStream))
                {
                    int filasSinFactura = 0;
                    int filasOtroProveedor = 0;
                    int filasFueraDelPeriodo = 0;
                    int filasOtroCanal = 0;

                    int filasProcesadas = 0;
                    int filasAceptadas = 0;
                    int filasRechazadas = 0;

                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed();
                    if (rows.Count() == 1)
                    {
                        return Json("El archivo no tiene datos.", JsonRequestBehavior.AllowGet);
                    }
                    /* limpiar tabla antes de cargar planilla leida */
                    despacho = new PlanillaZDIS022();
                    var i = despacho.truncDespachos("Despachos_TEMP"); // DespachosNormalizado - Despachos_PruebaDeCarga
                    Proveedor prov = null;
                    foreach (var row in rows.Skip(1))
                    {
                        if ((row.Cell(1).Value.ToString() != "") && (row.Cell(1).Value.ToString() != "Documento de venta"))
                        {
                            filasProcesadas = filasProcesadas + 1;
                            despacho = new PlanillaZDIS022();

                            string fechaEntregaOL_Mes = row.Cell(7).Value.ToString().Substring(3, 2);
                            string fechaEntregaOL_Ano = row.Cell(7).Value.ToString().Substring(6, 4);
                            string fechaEntregaOL = fechaEntregaOL_Ano + "-" + fechaEntregaOL_Mes;
                            string canal = row.Cell(4).Value.ToString();



                            if (OrganismoCanalesHelper.EsCanalValido(programaDefinido, canal))
                            {

                                if ((fechaEntregaOL == fechaCarga) || (!chkFiltroPeriodo))
                                {
                                    // el numero de factura se encuentra en la columna 13, por lo que validamos que sea distinto de cero 
                                    //int numfactura = int.Parse(row.Cell(13).Value.ToString());

                                    /* CONTROLAR VALORES NULOS EN FACTURAS */
                                    int numfactura;
                                    var valorCelda = row.Cell(13).Value;

                                    numfactura = int.TryParse(valorCelda.ToString(), out numfactura) ? numfactura : 0;

                                    // el pedido de compra se encuentra en la columna 2, lo usaremos para obtener el proveedor y asi validar la carga solicitada por el usuario del proveedor en filtro
                                    string pedidoCompra = row.Cell(2).Value.ToString();
                                    prov = new Proveedor();
                                    prov = prov.getProveedorPedidoCompra(pedidoCompra); // obtiene el rut del proveedor mediante el pedido de compra
                                    if (int.Parse(rutProveedorCarga) == (prov.RutProveedor)) // verifica que el rut obtenido corresponde al que se quiere cargar
                                    {
                                        if (numfactura > 0) // si existe número de factura se considera en la carga
                                        {
                                            filasAceptadas = filasAceptadas + 1;
                                            despacho = despacho._setRow(row);
                                            despacho.setDespachos(despacho, "Despachos_TEMP");
                                        }
                                        else
                                        {
                                            filasSinFactura = filasSinFactura + 1;
                                        }
                                    }
                                    else filasOtroProveedor = filasOtroProveedor + 1;
                                }
                                else { filasFueraDelPeriodo = filasFueraDelPeriodo + 1; }
                            }
                            else { filasOtroCanal = filasOtroCanal + 1; }
                        }
                    }
                    filasRechazadas = filasProcesadas - filasAceptadas;
                    return Json(new { message = "OK", status = true, 
                        count = filasProcesadas, 
                        countNoCero = filasAceptadas, 
                        count_Rechazos = filasRechazadas, 
                        count_filasFueraDelPeriodo = filasFueraDelPeriodo,
                        count_filasSinFactura = filasSinFactura,
                        count_filasOtroProveedor = filasOtroProveedor,
                        count_filasOtroCanal = filasOtroCanal,
                        count_FacturasAceptadas = filasAceptadas
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message, request);
            }
            return Json(new
            {
                message = "Error",
                status = false,
                count = 0,
                countNoCero = 0,
                countRechazos = 0,
                count_filasFueraDelPeriodo = 0,
                count_filasSinFactura = 0,
                count_filasOtroProveedor = 0,
                count_filasOtroCanal = 0
            });
        }
        public JsonResult consolidarUploadFile()
        {

            PlanillaZDIS022 despacho = new PlanillaZDIS022();
            var resp = despacho.consolidar();
            if (resp == "OK") return Json(new { message = "Los datos han sido consolidados exitosamente", status = true });
            else return Json(new { message = resp, status = false });

        }






    }
}