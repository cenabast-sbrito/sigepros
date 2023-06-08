using iTextSharp.text;
using iTextSharp.text.pdf;
using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

using System.Configuration;


namespace pnacpacam.Controllers
{
    [Authorize]
    [SessionExpire]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class DocumentosController : Controller
    {
        public JsonResult GetTipoDocumentos()
        {
            TipoDocumento doc = new TipoDocumento();

            var listaDocumentos = doc.GetTipoDocumentos();

            return Json(listaDocumentos, JsonRequestBehavior.AllowGet);

        }
        
        public JsonResult getDocumentos() {

            Documento cdc = new Documento();
            var listCdc = cdc.getDocumentos();

            return Json(listCdc, JsonRequestBehavior.AllowGet);

        }
        
        public JsonResult GetReferencias()
        {
            Referencia doc = new Referencia();

            var listaReferencias = doc.getReferencias();

            return Json(listaReferencias, JsonRequestBehavior.AllowGet);

        }

        // Obtener la lista de los inventarios creados  
        /*
        public JsonResult getListInventario()
        {

            modeloInventario cdc = new modeloInventario();
            var listCdc = cdc.getListInventario();

            return Json(listCdc, JsonRequestBehavior.AllowGet);

        }
        
        */
        /*
        public JsonResult getInventario(int IdInventario)
        {
            modeloInventario inventario = new modeloInventario();
            inventario = inventario.getInventario(IdInventario);
            return Json(inventario, JsonRequestBehavior.AllowGet);
        }
        */
        /*
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
                    return Json(new { message = "La cabecera del Inventario ha sido modificado exitosamente.", status = true });
                }
                else
                {
                    return Json(new { message = "Error : La cabecera del Inventario no ha podido ser modificada. ", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error : al intentar modificar la cabecera del Inventario. Descripcion : " + ex.Message + " " + ex.InnerException.Message, status = false });
            }
        }
        */
        
      /*  
        
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
                    return Json(new { message = "La cabecera del Inventario ha sido modificado exitosamente.", status = true });
                }
                else
                {
                    return Json(new { message = "Error : La cabecera del Inventario no ha podido ser modificada. ", status = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error : al intentar modificar la cabecera del Inventario. Descripcion : " + ex.Message + " " + ex.InnerException.Message, status = false });
            }
        }
    */
     /*   
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
    */
    }
}
