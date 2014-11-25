using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Ads.Code.Membership;
using App.Ads.Models;
using System.Web.Routing;

namespace App.Ads.Controllers
{
    public class BaseController : Controller
    {
        // Current logged in User Cache Data
        public CustomPrincipal CurrentUser = null;

        // For Ajax
        public ResponseModel responseModel { get; set; }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = User.ToCustomPrincipal();
            }

            ViewBag.SiteTitle = "Malaysia Free Classified Ads";
            ViewBag.Title = "Malaysia Free Classified Ads";
            ViewBag.SiteName = "9street.my";
        }

    }
}