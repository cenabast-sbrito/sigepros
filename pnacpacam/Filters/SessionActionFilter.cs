using System;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using ActionFilterAttribute = System.Web.Http.Filters.ActionFilterAttribute;

namespace PCSII.Filters
{
    //[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class SessionActionFilterAttribute : ActionFilterAttribute
    {

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;
            if (HttpContext.Current.Session["Rut"] == null)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                var loginUrl = url.Content("~/Index/Index");
                filterContext.HttpContext.Response.Redirect(loginUrl, true);
                
            }
        }  
    }
}