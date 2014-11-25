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
                "Error - 404",
                "PageNotFound",
                new { controller = "Exception", action = "PageNotFound" }
                );

            routes.MapRoute(
                "Error - 500",
                "Error",
                new { controller = "Exception", action = "Error" }
                );

            routes.MapRoute(
                 name: "Search",
                 url: "listings/{LocationText}/{CategoryText}",
                 defaults: new { controller = "Search", action = "Index", LocationText = UrlParameter.Optional, CategoryText = UrlParameter.Optional }
             );

            //Ad Detail
            routes.MapRoute(
                name: "view-ad-detail",
                url: "ad/{id}/{adTitle}",
                defaults: new { controller = "Advertisement", action = "Detail", title = UrlParameter.Optional }
            );

            //Article Detail
            routes.MapRoute(
                name: "view-article-detail",
                url: "about/{title}/{id}",
                defaults: new { controller = "Article", action = "Detail", title = UrlParameter.Optional }
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