using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace pnacpacam.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class ConfigController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // 1. Validar sesión ASP.NET
            if (Session == null || Session.IsNewSession)
            {
                CerrarSesion(filterContext);
                return;
            }

            // 2. Validar cookie de Forms Authentication
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

            // Cerrar Forms Auth
            FormsAuthentication.SignOut();

            // Eliminar cookie de autenticación
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = System.DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            // Redirigir a login
            filterContext.Result = RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public ActionResult GetVersion()
        {
            return Content(
                "versión " + ConfigurationManager.AppSettings["pnacpacam_versionAPP"],
                "text/plain"
            );
        }
    }
}