using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using App.Ads.Code.Membership;

namespace App.Ads.Areas.Account.Controllers
{
    public class LogoffController : Controller
    {
        //
        // POST: /Account/LogOff
        CustomIdentity identity;

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index()
        {
            WebSecurity.Logout();

            var cacheKey = string.Format("UserData_{0}", User.ToCustomPrincipal().CustomIdentity.Name);

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                HttpRuntime.Cache.Remove(cacheKey);
            }

            return RedirectToAction("Index", "Login", new { area = "Account" });
        }
    }
}