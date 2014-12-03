using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using App.Ads.Code.Membership;
using App.Ads.Controllers;
using App.Core.Services;

namespace App.Ads.Areas.Account.Controllers
{
    public class LogoffController : BaseController
    {
        private readonly ICacheService cacheService;

        public LogoffController(ICacheService cacheService)
        {
            this.cacheService = DependencyResolver.Current.GetService<ICacheService>();
        }

        // POST: /Account/LogOff
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                cacheService.Clear(string.Format(CacheConstant.USERDATA + "{0}", CurrentUser.Identity.Name));
                WebSecurity.Logout();
            }
            return RedirectToAction("Index", "Login", new { area = "Account" });
        }
    }
}