using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Ads.Controllers;
using App.Ads.Areas.Account.Models;
using App.Ads.Areas.Account.BO;
using App.Core.Services;

namespace App.Ads.Areas.Account.Controllers
{
    public class ChangePasswordController : BaseController
    {
        private readonly IAccountBO accountBO;
        private readonly IUserService userService;

        public ChangePasswordController()
        {
            this.accountBO = DependencyResolver.Current.GetService<IAccountBO>();
            this.userService = DependencyResolver.Current.GetService<IUserService>();
        }

        // GET: Account/ChangePassword
        public ActionResult Index(int uid, Guid? guid)
        {
            if (!guid.HasValue)
            {
                return HttpNotFound();
            }

            var membership = userService.GetMembershipByPasswordVerificationToken(uid, guid.ToString());

            if (membership == null)
            {
                return HttpNotFound();
            }

            CreateNewPassword model = new CreateNewPassword();

            if (membership.PasswordVerificationTokenExpirationDate < DateTime.Now)
            {
                TempData["Message"] = "Token Expired.";
                return RedirectToAction("Error", "Alert", new { area = "" });
            }

            model.UserId = membership.UserId;
            model.PasswordVerificationToken = new Guid(membership.PasswordVerificationToken);

            return View(model);
        }

        // POST
        [HttpPost]
        public ActionResult Index(CreateNewPassword model)
        {
            if (ModelState.IsValid)
            {
                if (accountBO.CreateNewPassword(model))
                {
                    TempData["Message"] = MessageConstant.PASSWORD_CHANGED;
                    return RedirectToAction("Success", new { area = "Account" });
                }
                else
                {
                    ViewBag.ModelState = model.ModelState;
                }
            }
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult Success()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

    }
}