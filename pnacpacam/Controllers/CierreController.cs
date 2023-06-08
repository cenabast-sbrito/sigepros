using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pnacpacam.Controllers
{
    [Authorize]
    [SessionExpire]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class CierreController : Controller
    {
        // GET: IngresoResultados
        public JsonResult GetCdcs()
        {
            modeloInventario cdc = new modeloInventario();
//            var cdcList = cdc.GetCdcs(Session["Periodo"].ToString(), Session["StringConexion"].ToString());
            var cdcList = cdc.getListInventario();
            return Json(cdcList, JsonRequestBehavior.AllowGet);
        }
   

         




    }
}