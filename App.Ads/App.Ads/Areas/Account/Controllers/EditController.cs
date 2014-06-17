using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using App.Ads.Areas.Account.Models;
using App.Core.Services;
using App.Core.Data;
using App.Ads.Code.Membership;
using AutoMapper;
using App.Ads.Controllers;
using App.Ads.ViewModel;

namespace App.Ads.Areas.Account.Controllers
{
    [Authorize]
    public class EditController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IUserService userService;
        private readonly Service.IAccountService accountService;

        public EditController()
        {
            this.commonService = DependencyResolver.Current.GetService<ICommonService>();
            this.userService = DependencyResolver.Current.GetService<IUserService>();
            this.accountService = DependencyResolver.Current.GetService<Service.IAccountService>();
        }

        // GET: Account/Edit
        public ActionResult Index()
        {
            ViewBag.Salutation = commonService.GetSalutationSelectList();

            var userProfile = userService.GetUserProfile(base.identity.UserId);

            var model = new AccountEditViewModel()
            {
                Profile = Mapper.Map<UserProfile, EditUserProfile>(userProfile)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(AccountEditViewModel model)
        {
            model.ChangePassword.UserId = base.identity.UserId;

            if (accountService.ChangePassword(model.ChangePassword))
            {
                return RedirectToAction("Index", "Listing", new { area = "" });
            }

            ViewBag.Salutation = commonService.GetSalutationSelectList();

            ViewBag.ModelState = model.ChangePassword.ModelState;

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult SaveDetail(AccountEditViewModel model)
        {
            var userProfile = userService.GetUserProfile(identity.UserId);

            if (userProfile == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                userProfile = Mapper.Map<EditUserProfile, UserProfile>(model.Profile, userProfile);

                userService.Save(userProfile);

                return RedirectToAction("Index", "Listing", new { area = "" });
            }

            ViewBag.Salutation = commonService.GetSalutationSelectList();

            return View("Index", model);
        }
    }
}