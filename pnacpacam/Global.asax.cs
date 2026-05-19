using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace pnacpacam
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;

            FormsAuthenticationTicket ticket;
            try
            {
                ticket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            if (ticket == null || ticket.Expired) return;

            var identity = new GenericIdentity(ticket.Name);

            // 👉 ROLES (pueden ser múltiples separados por coma)
            var roles = ticket.UserData.Split(',');

            HttpContext.Current.User =
                new GenericPrincipal(identity, roles);
        }
    }

    // 🔒 Este filtro sigue siendo válido (solo sesión)
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["PNACPACAM_RutUsuario"] == null)
            {
                filterContext.Result = new RedirectResult("~/Index/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}


/*

using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;


namespace pnacpacam
{

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }

    public class SessionExpireAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["PNACPACAM_RutUsuario"] == null)
            {
                filterContext.Result = new RedirectResult("~/Index/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }

}
*/