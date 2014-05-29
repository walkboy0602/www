using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using App.Ads.Code.Filters;

namespace App.Ads
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "DefaultActionApi",
               routeTemplate: "api/{controller}/{action}/{id}",
               defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new ValidateModelAttribute());

            /// Add below to resolve error
            /// The 'ObjectContent`1' type failed to serialize the response body for content type application/json

            //GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(new System.Net.Http.Formatting.QueryStringMapping("xml", "true", "application/xml"));
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new System.Net.Http.Formatting.QueryStringMapping("json", "true", "application/json"));

            //GlobalConfiguration.Configuration.Formatters.Clear();
            //GlobalConfiguration.Configuration.Formatters.Add(new JsonNetFormatter(new JsonSerializerSettings()));

            // New code:
            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling =
                Newtonsoft.Json.PreserveReferencesHandling.Objects;

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();
        }
    }
}