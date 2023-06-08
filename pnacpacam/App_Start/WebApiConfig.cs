using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using pnacpacam.Security;

namespace pnacpacam
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //deshabilitar cors
            //var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors();

            // Configuración de rutas y servicios de API
            config.MapHttpAttributeRoutes();

            config.MessageHandlers.Add(new TokenValidationHandler());
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler());

            config.Routes.MapHttpRoute(
                name: "Firma Gob - CENABAST",
                routeTemplate: @"api/v1/{controller}/{id}",
               //routeTemplate: "{controller}/{action}/{id}",
               //defaults: new { controller = "Index", action = "Index",  id = RouteParameter.Optional}
               defaults: new { id = RouteParameter.Optional }
            );

            // Adding formatter for Json   
            config.Formatters.JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("type", "json", new MediaTypeHeaderValue("application/json")));

            // Adding formatter for XML   
            config.Formatters.XmlFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("type", "xml", new MediaTypeHeaderValue("application/xml")));
        
        }
    }
}
