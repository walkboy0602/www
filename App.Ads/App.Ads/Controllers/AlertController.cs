using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Ads.Controllers
{
    public class AlertController : BaseController
    {
        // GET: Alert
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Error()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }
    }
}