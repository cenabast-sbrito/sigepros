
using pnacpacam.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace pnacpacam.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("ADM"))
                return View("Index", "Admin/_AdminLayout");

            if (User.IsInRole("ADP"))
                return View("Index", "Admin/_AdminLayout");

            if (User.IsInRole("CON"))
                return View("Index", "perfilContrato/_ContratoLayout");

            if (User.IsInRole("COP"))
                return View("Index", "perfilContrato/_ContratoLayout");

            if (User.IsInRole("MIN"))
                return View("Index", "perfilMinsal/_MinsalLayout");

            if (User.IsInRole("MIP"))
                return View("Index", "perfilMinsal/_MinsalLayout");

            return RedirectToAction("Index", "Index");
        }

    /*
        public JsonResult GetDataUser()
        {
            Usuario user = new Usuario();
            user = user.GetUsuario(Session["PNACPACAM_RutUsuario"].ToString());
            return Json(user, JsonRequestBehavior.AllowGet);
        }
    */

        public void LogOut()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.RemoveAll();

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            Response.Cookies.Add(new System.Web.HttpCookie("ASP.NET_SessionId", ""));
            RedirectToAction("Index", "Index");
        }

    }
}
