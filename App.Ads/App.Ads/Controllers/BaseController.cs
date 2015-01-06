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

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ViewBag.Title = "Malaysia Free Classified Ads";
            ViewBag.Keywords = "Property For Sale, Car For Sale, Used Item, Used Stuff, Unwanted Stuff, Free Classified, Furniture For Sale, House Cleaning, Jobs";
            ViewBag.Description = "Looking to Buy, Sell stuff online? Search 9street.my Malaysia Free Classifieds for Free!";

            ViewBag.SiteName = "9street.my";
            ViewBag.SiteTitle = "Malaysia Free Classified Ads";

            ViewBag.OG_URL = Request.Url.AbsoluteUri;
            ViewBag.OG_Title = "Malaysia Free Classified Ads";
            ViewBag.OG_Desc = "Search thousands of stores, brands, products and user classifieds all in one place!";
            ViewBag.OG_Image = "https://bazaarstorage.blob.core.windows.net/image/common/fblogo3.jpg";
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = User.ToCustomPrincipal();
            }

        }

    }
}