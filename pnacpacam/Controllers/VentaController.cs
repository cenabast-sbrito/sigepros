using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using pnacpacam.Models;

namespace pnacpacam.Controllers
{
    public class VentaController : Controller
    {
        // GET: Venta
//        [System.Web.Http.HttpPost]
        public JsonResult addVenta(Venta venta)
        {
            venta.rutResponsable = Session["Rut"].ToString();
            ventaDetalle det = new ventaDetalle();
            int i = 0;
            i = venta.addVenta(venta);
            if ( i > 0 ) {

                if (venta.detalle != null)
                {
                    i = det.addVentaDetalle(venta.detalle, i);
                }
                return Json(i, JsonRequestBehavior.AllowGet);

            } else {

                return Json(i, JsonRequestBehavior.AllowGet);

            }

        }
        public JsonResult getVentas()
        {

            Venta venta = new Venta();
            var lista = venta.getVentas();

            return Json(lista, JsonRequestBehavior.AllowGet);

        }

    }
}


