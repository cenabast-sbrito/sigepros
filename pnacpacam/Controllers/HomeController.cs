
using System.Web.Mvc;
using System;
using System.Web;
using System.Web.Security;
using pnacpacam.Models;


namespace pnacpacam.Controllers
{
    [Authorize]
    [SessionExpire]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            if (Session["Rut"]!= null)
            {
                if (Session["Perfil"].ToString() == "1")
                {
                    return View("Index", "Admin/_AdminLayout"); // sin menu reportabilidad

                }
                else if (Session["Perfil"].ToString() == "2")
                {
                    return View("Index", "administrador/_AdministradorLayout"); // con menu reportabilidad
                }
                else if (Session["Perfil"].ToString() == "3")
                {
                    return View("Index", "EjecutivoUnidadGestora/_UnidadGestoraLayout");
                }
                else if (Session["Perfil"].ToString() == "6")
                {
                    return View("Index", "Visitante/_VisitanteLayout");
                }
                else {
                    return View("Index", "EjecutivoUnidadGestora/_Layout");
                }
            }
            else {
                return RedirectToAction("Index", "Index");
            }
            
        }

        public JsonResult GetDataUser() {
            //Funcionario fun = new Funcionario();
            UsuarioViewModelModify user = new UsuarioViewModelModify();
            user = user.GetUsuario(Session["Rut"].ToString(), Session["StringConexion"].ToString());
            /*
             * dynamic dataUser = new ExpandoObject();
            dataUser.funcionario = fun.GetFuncionario(Session["Rut"].ToString());
            dataUser.user = user.GetUsuario(Session["Rut"].ToString());
            */
            return Json( user, JsonRequestBehavior.AllowGet);
        }

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
