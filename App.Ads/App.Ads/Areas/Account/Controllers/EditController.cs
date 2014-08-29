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
using App.Ads.Code.Constant;

namespace App.Ads.Areas.Account.Controllers
{
    [Authorize]
    public class EditController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IUserService userService;
        private readonly ICacheService cacheService;
        private readonly Service.IAccountService accountService;

        public EditController()
        {
            this.commonService = DependencyResolver.Current.GetService<ICommonService>();
            this.userService = DependencyResolver.Current.GetService<IUserService>();
            this.cacheService = DependencyResolver.Current.GetService<ICacheService>();
            this.accountService = DependencyResolver.Current.GetService<Service.IAccountService>();
        }

        // GET: Account/Edit
        public ActionResult Index()
        {
            ViewBag.Salutation = commonService.GetSalutationSelectList();

            var userProfile = userService.GetUserProfile(CurrentUser.CustomIdentity.UserId);

            var model = new AccountEditViewModel()
            {
                Profile = Mapper.Map<UserProfile, EditUserProfile>(userProfile)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(AccountEditViewModel model)
        {
            model.ChangePassword.UserId = CurrentUser.CustomIdentity.UserId;

            if (accountService.ChangePassword(model.ChangePassword))
            {
                cacheService.Clear(string.Format(CacheConstant.USERDATA + "{0}", CurrentUser.Identity.Name));
                ViewBag.SuccessMsg = MessageConstant.PASSWORD_CHANGED;
            }
            else
            {
                ViewBag.ModelState = model.ChangePassword.ModelState;
            }

            ViewBag.Salutation = commonService.GetSalutationSelectList();

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult SaveDetail(AccountEditViewModel model)
        {
            var userProfile = userService.GetUserProfile(CurrentUser.CustomIdentity.UserId);

            if (userProfile == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                userProfile = Mapper.Map<EditUserProfile, UserProfile>(model.Profile, userProfile);

                userService.Save(userProfile);

                cacheService.Clear(string.Format(CacheConstant.USERDATA + "{0}", CurrentUser.Identity.Name));
                ViewBag.SuccessMsg = MessageConstant.ACCOUNT_DETAIL_CHANGED;
            }

            ViewBag.Salutation = commonService.GetSalutationSelectList();

            return View("Index", model);
        }
    }
}