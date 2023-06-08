using pnacpacam.Models;
using PNacPacam;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pnacpacam.Controllers
{
    public class ProveedorController : Controller
    {
        // GET: Proveedor
        public JsonResult getProveedores()
        {
/*            Proveedor proveedor = new Proveedor();
            proveedor = proveedor.getProveedor();
            return Json(proveedor, JsonRequestBehavior.AllowGet);*/

            Proveedor inventario = new Proveedor();
            var list = inventario.getListProveedor();
            return Json(list, JsonRequestBehavior.AllowGet);


        }
        public JsonResult getDespachos(string rutProveedor)
        {
            

            Despachos proveedor = new Despachos();

            //var x = proveedor.unirPDF();

            var listCdc = proveedor.getDespachos(rutProveedor);

            return Json(listCdc, JsonRequestBehavior.AllowGet);

        }
    }

   
}