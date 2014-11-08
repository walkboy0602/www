using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using App.Ads.Areas.Account.Models;
using App.Ads.Controllers;

namespace App.Ads.Areas.Account.Controllers
{
    public class LoginController : BaseController
    {
        //
        // GET: /Account/Login/
        public ActionResult Index(string returnUrl)
        {
            ViewBag.Title = "Login";
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "User", new { area = "" });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult Index(LoginModel model, string returnUrl)
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Dashboard", "User", new { area = "" });
            }

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return Url.IsLocalUrl(returnUrl) ? (ActionResult)Redirect(returnUrl) : RedirectToAction("Dashboard", "User", new { area = "" });
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }
    }
}