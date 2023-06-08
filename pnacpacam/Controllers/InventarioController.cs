using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static pnacpacam.modeloProducto;

namespace pnacpacam.Controllers
{
    [Authorize]
    [SessionExpire]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class InventarioController : Controller
    {


        public JsonResult getListInventario()
        {

            modeloInventario cdc = new modeloInventario();
            var listCdc = cdc.getListInventario();

            return Json(listCdc, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getTablaInventario(int IdInventario)
        {
            modeloInventario inventario = new modeloInventario();
            inventario = inventario.getInventario(IdInventario);
            return Json(inventario, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult SetInventario(modeloInventario inventario)
        {
            try
            {
                if (Session["Rut"] != null)
                {
                    inventario.RutCreador = Session["Rut"].ToString();
                }
                else
                {
                    return Json(new { message = "Su sesion ha expirado. Vuelva a Conectarse", status = false });
                }
                if (inventario.setInventario(inventario) > 0)
                {
                    return Json(new { message = "La cabecera del pnacpacam ha sido modificado exitosamente.", status = true });
                }
                else
                {
                    return Json(new { message = "Error : La cabecera del pnacpacam no ha podido ser modificada. ", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error : al intentar modificar la cabecera del pnacpacam. Descripcion : " + ex.Message + " " + ex.InnerException.Message, status = false });
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        public JsonResult PutInventario(modeloInventario inventario)
        {
            try
            {
                if (Session["Rut"] != null)
                {
                    inventario.RutCreador = Session["Rut"].ToString();
                }
                else
                {
                    return Json(new { message = "Su sesion ha expirado. Vuelva a Conectarse", status = false });
                }
                if (inventario.putInventario(inventario) > 0)
                {
                    return Json(new { message = "La cabecera del pnacpacam ha sido modificado exitosamente.", status = true });
                }
                else
                {
                    return Json(new { message = "Error : La cabecera del pnacpacam no ha podido ser modificada. ", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error : al intentar modificar la cabecera del pnacpacam. Descripcion : " + ex.Message + " " + ex.InnerException.Message, status = false });
            }
        }

        public int GetSession()
        {
            if (Session["Rut"] != null)
            {

                return 1;

            }
            else
            {
                return 0;
            }
        }
        public string GetStringConexion()
        {
            if (Session["Rut"] != null)
            {
                return ConfigurationManager.AppSettings["StringConexion"].ToString();
            }
            else
            {
                return "";
            }
        }
        public string GetPeriodo()
        {
            if (Session["Rut"] != null)
            {
                return Session["Periodo"].ToString();
            }
            else
            {
                return "";
            }
        }

        public JsonResult getListaInventarioDetalle(int idInventario)
        {
            InventarioDetalle inventarioDetalle = new InventarioDetalle();
            var list = inventarioDetalle.getInventarioDetalleV2(idInventario);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult putInventarioDetalleCantidad(int IdInventario, string CodigoProducto, string Cantidad, string Serie)
        {
            try
            {
                InventarioDetalle inventarioDetalle = new InventarioDetalle();
                inventarioDetalle.idInventario = IdInventario;
                inventarioDetalle.codigoProducto=CodigoProducto;
                inventarioDetalle.cantidad = Cantidad;
                inventarioDetalle.serie = Serie;
                inventarioDetalle.idSerie = CodigoProducto+Serie;
                if (inventarioDetalle.putInventarioDetalle(inventarioDetalle)>0)
                {
                    return Json(new { message = "El resultado se ha registrado exitosamente.", status = true });
                }
                else
                {
                    if (inventarioDetalle.setInventarioDetalle(inventarioDetalle) > 0)
                    {
                        return Json(new { message = "El resultado se ha actualizado exitosamente.", status = true });
                    }
                    else
                        return Json(new { message = "No ha sido posible actualizar cantidad en el inventario.", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Ha ocurrido un error al actualizar cantidad en el inventario. " + ex.Message, status = false });
            }
        }
        public JsonResult getListaInventarios()
        {
            modeloInventario inventario = new modeloInventario();
            var list = inventario.getListInventario();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getReporteInventario(int queMostrar)
        {
            Reportes inventario = new Reportes();
            var list = inventario.getReporteInventario(queMostrar);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getInventario(int id)
        {
            modeloInventario inventario = new modeloInventario();
            var list = inventario.getInventario(id);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCierreDetalle(string mes, string ano)
        {
            modeloCierre inventarioDetalle = new modeloCierre();
            var list = inventarioDetalle.getCierreDetalle(mes,ano);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getCierre(string mes, string ano)
        {
            sp_cierreMensual_From_Reporte inventarioDetalle = new sp_cierreMensual_From_Reporte();
            var list = inventarioDetalle.getPreviewCierre(mes, ano);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult importarInventario( string fichero )
        {
            FicheroDeCarga inventario = new FicheroDeCarga();
            var list = inventario.importarInventario( fichero );
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult asignarInventario(string fichero, int idInventario)
        {
            FicheroDeCarga inventario = new FicheroDeCarga();
            var list = inventario.asignarInventario(fichero, idInventario);

            return Json(list, JsonRequestBehavior.AllowGet); 
        }

        public JsonResult getListaProductos()
        {
            modeloProducto productos = new modeloProducto();
            var lista = productos.getProductos();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getListaProductosSerie()
        {
            modeloProducto productos = new modeloProducto();
            var lista = productos.getProductosSerie();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getListaClientes()
        {
            Cliente clientes = new Cliente();
            var lista = clientes.getListCliente();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getListaSeriesProducto(string codigoProducto)
        {
            productoSeries productos = new productoSeries();
            var lista = productos.getSeriesProductos(codigoProducto);
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getProducto(string codigoProducto)
        {
            modeloProducto producto = new modeloProducto();
            var prod = producto.getProducto(codigoProducto);
            return Json(prod, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getUnidadVenta(string codigoProducto)
        {
            unidadMedida um = new unidadMedida();
            var u = um.getUnidadVenta(codigoProducto);
            return Json(u, JsonRequestBehavior.AllowGet);
        }
        /* MOVIMIENTOS DEL INVENTARIO */
        public JsonResult getMovimientos()
        {
            Movimiento mov = new Movimiento();
            var lista = mov.getMovimientos();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SetMovimiento(Movimiento movimiento)
        {
            try
            {
                if (Session["Rut"] != null)
                {
                    movimiento.rutResponsable = Session["Rut"].ToString();
                }
                else
                {
                    return Json(new { message = "Su sesion ha expirado. Vuelva a Conectarse", status = false });
                }
                if (movimiento.setMovimiento(movimiento) > 0)
                {
                    return Json(new { message = "El movimiento ha sido registrado de manera exitosa.", status = true });
                }
                else
                {
                    return Json(new { message = "Error : El movimiento no ha podido ser registrado. ", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error : al intentar registrar el movimiento. Descripcion : " + ex.Message + " " + ex.InnerException.Message, status = false });
            }
        }
        public JsonResult getBodegas()
        {
            Bodega bodega = new Bodega();
            var lista = bodega.getBodegas();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getListaBodegasProducto(string codigoProducto, string serie)
        {
            productoBodega productos = new productoBodega();
            var lista = productos.getBodegasProducto(codigoProducto, serie);

            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getListaBodegas()
        {
            productoBodega productos = new productoBodega();
            var lista = productos.getBodegas();

            return Json(lista, JsonRequestBehavior.AllowGet);

        }

        /* FIN MOVIMIENTOS DEL INVENTARIO */


    }
}










