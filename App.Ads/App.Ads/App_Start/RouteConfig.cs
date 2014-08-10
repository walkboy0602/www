using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace App.Ads
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                 name: "Search",
                 url: "listings/{category}",
                 defaults: new { controller = "Search", action = "Index", category = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "view-ad-detail",
                url: "ad/{id}/{adTitle}",
                defaults: new { controller = "Advertisement", action = "index", title = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "user-dashboard",
                url: "dashboard",
                defaults: new { controller = "User", action = "Dashboard" }
             );

            routes.MapRoute(
                    "Default",
                    "{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new[] { "App.Ads.Controllers" }
             );


        }
    }
}