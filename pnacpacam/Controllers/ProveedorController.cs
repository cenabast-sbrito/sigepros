
using ClosedXML.Excel;
using pnacpacam.Models;
using PNacPacam;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static PNacPacam.Despachos;

namespace pnacpacam.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ProveedorController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1️⃣ Sesión ASP.NET expirada
            if (Session == null || Session.IsNewSession)
            {
                CerrarSesionJson(filterContext);
                return;
            }

            // 2️⃣ Forms Authentication inválido
            if (!Request.IsAuthenticated || User == null || !User.Identity.IsAuthenticated)
            {
                CerrarSesionJson(filterContext);
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private void CerrarSesionJson(ActionExecutingContext filterContext)
        {
            // Limpiar sesión
            Session.Clear();
            Session.Abandon();

            // Cerrar Forms Auth
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

            // Respuesta JSON para el front
            filterContext.HttpContext.Response.StatusCode = 401;
            filterContext.Result = Json(new
            {
                state = false,
                message = "Sesión Finalizada"
            }, JsonRequestBehavior.AllowGet);
        }

        //        [Authorize]
        public JsonResult getFacturas(string rutProveedor, string nFactura, string anio, string nEstado)
        {
            try
            {
                string rol = User.IsInRole("ADM") ? "ADM" :
                             User.IsInRole("MIN") ? "MIN" :
                             User.IsInRole("MIP") ? "MIP" :
                             User.IsInRole("ADP") ? "ADP" :
                             User.IsInRole("CON") ? "CON" :
                             User.IsInRole("COP") ? "COP" : "";

                Despachos.Factura factura = new Despachos.Factura();

                var facturas = factura._getFacturas(
                    rutProveedor,
                    nFactura,
                    anio,
                    nEstado,
                    rol
                );

                return Json(new
                {
                    state = true,
                    message = "OK.",
                    resultado = facturas
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new
                {
                    state = false,
                    message = "Sin Datos"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SetProveedor(Proveedor proveedor)
        {
            string mensaje;
            int resultado = proveedor.Ingresar(proveedor, out mensaje);

            if (resultado > 0)
            {
                return Json(new
                {
                    status = true,
                    message = "Se ha agregado el proveedor exitosamente."
                });
            }

            return Json(new
            {
                status = false,
                message = string.IsNullOrEmpty(mensaje)
                    ? "Ha ocurrido un error."
                    : mensaje
            });
        }
        public JsonResult PutProveedor(Proveedor proveedor)
        {
            int i = proveedor.Actualizar(proveedor);
            if (i > 0)
            {
                return Json(new { status = true, message = "Se ha actualizado el Proveedor exitosamente." });
            }
            else
            {
                return Json(new { status = false, message = "Ha ocurrido un error." });
            }
        }

        public JsonResult GetProveedor(int programa, string rut)
        {//sbritousuarios
            Proveedor proveedor = new Proveedor();
            proveedor = proveedor.getProveedor(programa, rut);
            return Json(proveedor, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DelProveedor(string rut)
        {//sbritousuarios
            Proveedor proveedor = new Proveedor();
            int i = proveedor.delProveedor(rut);
            return Json(i, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddProveedor(string rut)
        {//sbritousuarios
            Proveedor proveedor = new Proveedor();
            int i = proveedor.addProveedor(rut);
            return Json(i, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProveedores(string tipoConsulta)
        {

            int programa = User.IsInRole("ADM") ? 0 :
                           User.IsInRole("MIN") ? 0 :
                           User.IsInRole("MIP") ? 1 :
                           User.IsInRole("ADP") ? 1 :
                           User.IsInRole("CON") ? 0 :
                           User.IsInRole("COP") ? 1 : 1;
            ;
            Proveedor inventario = new Proveedor();
            var list = inventario.getListProveedorPnacPacam(tipoConsulta, programa);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getDespachosByFactura(string rut, string factura, Boolean incluye)
        {
            Despachos despacho = new Despachos();
            List<Despachos> despachos = new List<Despachos>();
            try
            {
                despachos = despacho.getDespachosByFactura(
                    rut,
                    factura,
                    incluye
                    );

                if (despachos == null)
                    return Json(new
                    {
                        state = false,
                        message = " Lista vacía, solicite a TI revisar tabla SAP VBPA ",
                        resultado = despachos
                    }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    state = true,
                    message = "OK.",
                    resultado = despachos
                }, JsonRequestBehavior.AllowGet);
            }
            catch (SqlException ex)
            {
                string mensaje = ex.Number == -2
                    ? "Se agotó el tiempo de espera en la consulta. La Base de Datos No Respondió. Intente más tarde."
                    : "Error en la base de datos.";

                return Json(new
                {
                    state = false,
                    message = mensaje,
                    resultado = (object)null
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    state = false,
                    message = "Sesión Finalizada",
                    resultado = despacho
                }, JsonRequestBehavior.AllowGet);

            }
        }
        public JsonResult getListFacturas(string rutproveedor)
        {
            FacturaLista facturaLista = new FacturaLista();
            var list = facturaLista.getListFactura(rutproveedor);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ObtenerDocumentos(string rut, string doc)
        {
            try
            {
                string url = ConfigurationManager.AppSettings["urlCargaSapPro"].ToString();
                string urlDA = ConfigurationManager.AppSettings["urlBaseLinkDocumentos"].ToString();
                string urlIFrame = ConfigurationManager.AppSettings["urlIFRAME"].ToString();
                var urlBaseCedibles = ConfigurationManager.AppSettings["urlBaseCedibles"].ToString();
                string linkCedible = urlBaseCedibles + int.Parse(rut) + "/procesados/" + doc + ".pdf";
                string folder = urlBaseCedibles + int.Parse(rut) + "/procesados/";
                folder = url + int.Parse(rut) + "/procesados/";

                Archivo archivo = new Archivo();
                List<Archivo> archivos = new List<Archivo>();

                string file = doc + ".pdf";
                //string fileFind = doc + "*.pdf";
                string fileFind = doc == "*" ? "*.pdf" : doc + "*.pdf";

                linkCedible = folder + file;

                var archivosLista = Directory.GetFiles(folder, fileFind)
                    .Select(Path.GetFileName)
                    .OrderBy(f => f)
                    .Select(f => new Archivo { Nombre = f })
                    .ToList();

                var modelo = new Archivos
                {
                    Url = urlIFrame + int.Parse(rut) + "/procesados/",
                    UrlDA = urlDA + int.Parse(rut) + "/",
                    Nombres = archivosLista
                };

                //return modelo.Nombres;

                return Json(new { state = true, archivos = modelo }, JsonRequestBehavior.AllowGet);
                /*
                    Archivo archivo = new Archivo();
                    List<Archivo> archivos = new List<Archivo>();
                    try
                    {
                        archivos = archivo.GetArchivos(rut, doc);

                        return Json(new { state = true, archivos }, JsonRequestBehavior.AllowGet);


                    } 
                */
            }
            catch (Exception e)
            {
                return Json(new { state = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult getCountVBPA()
        {
            /// desactive esta funcion para mejorar los tiempo de respuesta
            return Json(new { state = true, message = "OK" }, JsonRequestBehavior.AllowGet);
            // /* prueba de tabla vacia */ return Json(new { state = false, message = " Tabla SAP 'VBPA' se encuentra vacía, favor informar a TI " }, JsonRequestBehavior.AllowGet);
            Movimiento doc = new Movimiento();
            int statusTableSAP = doc._getCountSAPTable_VBPA();
            if (statusTableSAP == 0)
            {
                return Json(new { state = false, message = " Tabla SAP 'VBPA' se encuentra vacía, favor informar a TI " }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { state = true, message = "OK" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult crearExpediente(string rutProveedor, string factura)
        {
            Movimiento doc = new Movimiento();
            // int carga = doc.porcentajeCargaExpediente(rutProveedor, factura);
            // if (carga < 100) { return Json("carga incompleta de los expedientes", JsonRequestBehavior.AllowGet); }

            int statusTableSAP = doc._getCountSAPTable_VBPA();
            if (statusTableSAP == 0)
            {
                return Json(new { state = false, message = " Tabla SAP 'VBPA' se encuentra vacía, favor informar a TI " }, JsonRequestBehavior.AllowGet);
            }
            var resultado = doc.crearExpediente(rutProveedor, factura);
            /*Marcar en el estado factura el expediente armado con fecha*/

            if (resultado == "OK")
            {
                if (doc.existeExpediente(rutProveedor, factura, true) == 1)
                {
                    EstadoFactura estado = new EstadoFactura();
                    estado = estado.putEstado("", rutProveedor, factura, "Expediente", "");
                }
                return Json(new { state = true, message = "OK" }, JsonRequestBehavior.AllowGet);
            }
            else { return Json(new { state = false, message = resultado }, JsonRequestBehavior.AllowGet); }

            //return Json(listaDocumentos, JsonRequestBehavior.AllowGet);

        }

        public Boolean existeExpediente(string rutProveedor, string factura)
        {
            Movimiento doc = new Movimiento();
            if (doc.existeExpediente(rutProveedor, factura, true) == 1)
            {
                return true;
            }
            return false;

        }
        public Boolean existeFacturaComision(string rutProveedor, string factura)
        {
            //            return true;
            Movimiento doc = new Movimiento();
            if (doc.existeFacturaComision(rutProveedor, factura) == 1)
            {
                return true;
            }
            return false;

        }
        public Boolean existeDocumentoAdjunto(string rutProveedor, string factura)
        {
            //            return true;
            Movimiento doc = new Movimiento();
            if (doc.ExisteDocumentoAdjunto(rutProveedor, factura) == 1)
            {
                return true;
            }
            return false;

        }

        public JsonResult getFacturasHistoricas(string rutProveedor, string nFactura, string anio, string estado)
        {

            Despachos.Factura factura = new Despachos.Factura();
            List<Factura> facturas = new List<Factura>();
            try
            {
                facturas = factura._getFacturasHistoricas(rutProveedor, nFactura, anio, estado, Session["pnacpacam_Rol"].ToString());
                if (facturas == null)
                    return Json(new { state = false, message = "Sesión Finalizada", resultado = factura }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { state = true, message = "OK.", resultado = facturas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { state = false, message = "Sesión Finalizada", resultado = factura }, JsonRequestBehavior.AllowGet);
            }

        }
        public Factura getFactura(string rutProveedor, string nFactura, string anio, string estado)
        {

            Despachos.Factura factura = new Despachos.Factura();
            try
            {
                factura = factura._getFactura(rutProveedor, nFactura, anio, estado, Session["pnacpacam_Rol"].ToString());
                return factura;
            }
            catch (Exception)
            {
                return factura;
            }

        }
        public JsonResult getEstados()
        {
            string rol = User.IsInRole("ADM") ? "ADM" :
                         User.IsInRole("MIN") ? "MIN" :
                         User.IsInRole("MIP") ? "MIP" :
                         User.IsInRole("ADP") ? "ADP" :
                         User.IsInRole("CON") ? "CON" :
                         User.IsInRole("COP") ? "COP" : "";

            Estado estado = new Estado();

            var estados = estado._getEstados(rol); //Session["pnacpacam_Rol"].ToString()

            return Json(estados, JsonRequestBehavior.AllowGet);

        }

        public JsonResult putEstado(string rutUsuario, string rutProveedor, string NFactura, string estadoActual, string estadoNuevo, string obs)
        {
            try
            {

                if (estadoNuevo == "4")
                {
                    string ambiente = ConfigurationManager.AppSettings["Ambiente"];
                    var rutaBaseFisicaExpedientes = (ambiente == "Production") ? ConfigurationManager.AppSettings["urlBaseIOExpedientes"].ToString() :
                                                                 ConfigurationManager.AppSettings["urlBaseIOExpedientes_TEST"].ToString();

                    // ===================== EXPEDIENTE =====================
                    string nombreExpediente = $"{rutProveedor}_{NFactura}_FULL.pdf";
                    string rutaExpediente = Path.Combine(rutaBaseFisicaExpedientes, nombreExpediente);

                    if (!System.IO.File.Exists(rutaExpediente)) throw new ArgumentException("El Expediente no ha sido creado aún. No es posible enviar a MINSAL");
                }

                // Actualizar estado
                EstadoFactura estado = new EstadoFactura();
                estado = estado.putEstado(rutUsuario, rutProveedor, NFactura, estadoNuevo, obs);

                // Registrar log
                Log log = new Log();
                log.addLog(rutUsuario, rutProveedor, int.Parse(NFactura), estadoActual, estadoNuevo);

                // Obtener la factura actualizada
                Despachos.Factura facturaObj = new Despachos.Factura();
                var facturaActualizada = facturaObj._getFactura(
                    rutProveedor,
                    NFactura,
                    "",
                    "",
                    Session["pnacpacam_Rol"].ToString()
                );

                // Devolver JSON estructurado
                return Json(new
                {
                    success = true,
                    message = "Estado actualizado correctamente",
                    resultado = facturaActualizada
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Capturar cualquier error y devolverlo como JSON
                return Json(new
                {
                    success = false,
                    message = "Error al actualizar el estado: " + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult putEstado_1(string rutUsuario, string rutProveedor, string NFactura,
            string estadoActual, string estadoNuevo, string obs)
        {
            Despachos.Factura factura = new Despachos.Factura(); // quiero que me devuekva el estado en el que quedo parta enviarlo al front y poder pintarlo

            EstadoFactura estado = new EstadoFactura();
            estado = estado.putEstado(rutUsuario, rutProveedor, NFactura, estadoNuevo, obs);

            Log log = new Log();
            var i = log.addLog(rutUsuario, rutProveedor, int.Parse(NFactura), estadoActual, estadoNuevo);

            return Json(factura = factura._getFactura(rutProveedor, NFactura, "", "", Session["pnacpacam_Rol"].ToString()), JsonRequestBehavior.AllowGet);
            // return Json(estado, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getNotificaciones(string rutProveedor)
        {

            Notificacion factura = new Notificacion();

            var facturas = factura.getNotificaciones(rutProveedor);

            return Json(facturas, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getProgramas()
        {

            Programa programa = new Programa();

            var facturas = programa.getListProgramas();

            return Json(facturas, JsonRequestBehavior.AllowGet);

        }



    }

}