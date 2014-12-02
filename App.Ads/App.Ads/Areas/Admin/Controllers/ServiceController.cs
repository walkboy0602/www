using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Ads.Code.Security;

namespace App.Ads.Areas.Admin.Controllers
{
    [Authorize]
    public class ServiceController : Controller
    {
        // GET: Admin/Service
        public ActionResult Index()
        {
            return View();
        }

        [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult ClearCache()
        {
            List<string> keys = new List<string>();

            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = HttpRuntime.Cache.GetEnumerator();

            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }

            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                HttpRuntime.Cache.Remove(keys[i]);
            }

            @ViewBag.Message = "Successfully removed all cache.";

            return View();
        }
    }
}