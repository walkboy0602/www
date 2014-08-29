using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Core.Services;
using App.Ads.ViewModel;
using App.Ads.Areas.Account.Models;
using App.Core.Helper;

namespace App.Ads.Areas.Account.Service
{
    public interface IAccountService
    {
        bool ChangePassword(ChangePassword model);
    }

    public class AccountService : IAccountService
    {
        private readonly IUserService _userService;
 
        public AccountService(IUserService userService)
        {
            this._userService = userService;
        }

        public bool ChangePassword(ChangePassword model)
        {
            var membership = _userService.GetMembership(model.UserId);
            if (membership == null)
            {
                model.ModelState.AddModelError("UserName", "Invalid Username");
                return false;
            }

            if (membership.PasswordSalt != _userService.GetHash(model.OldPassword))
            {
                model.ModelState.AddModelError("OldPassword", "Invalid old password.");
                return false;
            }

            membership.PasswordSalt = _userService.GetHash(model.Password);
            membership.PasswordChangedDate = DateTime.Now;
            _userService.Save(membership);
            return true;
        }

    }
}