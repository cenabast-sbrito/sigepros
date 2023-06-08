using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static pnacpacam.Models.Compra;

namespace pnacpacam.Controllers
{
    public class CompraController : Controller
    {
        // GET: Compra

        public JsonResult setCompra(Compra compra)
        {
            try
            {
                if (Session["Rut"] != null)
                {
                    compra.rutResponsable = Session["Rut"].ToString();
                }
                else
                {
                    return Json(new { message = "Su sesion ha expirado. Vuelva a Conectarse", status = false });
                }
                if (compra.setCompra(compra) > 0)
                {
                    if (compra.setCompraDetalle(compra) > 0)
                    {
                        return Json(new { message = "El documento ha sido guardado exitosamente.", status = true });
                    }
                    else
                    {
                        return Json(new { message = "Error : Ocurrió algún error, el detalle del documento no fue guardado. ", status = false });
                    }
                }
                else
                {
                    return Json(new { message = "Error : Ocurrió algún error, el documento no fue guardado. ", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error : al intentar modificar el Documento. detalle : " + ex.Message + " " + ex.InnerException.Message, status = false });
            }
        }

        public JsonResult getListCompras()
        {

            Compra compras = new Compra();
            var lista = compras.getCompras();

            return Json(lista, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getDetalleCompras()
        {
            compraDetalle dc = new compraDetalle();
            var lista = dc.getDetalleCompra();
            return Json(lista, JsonRequestBehavior.AllowGet);

        }
    }
}