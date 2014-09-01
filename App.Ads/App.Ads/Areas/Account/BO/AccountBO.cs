using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Core.Services;
using App.Ads.ViewModel;
using App.Ads.Areas.Account.Models;
using App.Core.Helper;

namespace App.Ads.Areas.Account.BO
{
    public interface IAccountBO
    {
        bool ChangePassword(ChangePassword model);

        bool ForgotPassword(ForgotPasswordModel model);
    }

    public class AccountBO : IAccountBO
    {
        private readonly IUserService _userService;
 
        public AccountBO(IUserService userService)
        {
            this._userService = userService;
        }

        bool IAccountBO.ChangePassword(ChangePassword model)
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

        bool IAccountBO.ForgotPassword(ForgotPasswordModel model)
        {
            var membership = _userService.GetMembership(model.Email);
            if (membership == null)
            {
                model.ModelState.AddModelError("Email", "Invalid email address.");
                return false;
            }

            if (!string.IsNullOrEmpty(membership.PasswordVerificationToken) 
                && DateTime.Compare((DateTime)membership.PasswordVerificationTokenExpirationDate, DateTime.Now) > 0)
            {
                //Password requested before
                return true;
            }

            membership.PasswordVerificationToken = Guid.NewGuid().ToString().ToLower();
            membership.PasswordVerificationTokenExpirationDate = DateTime.Now.AddDays(1); // Expired in 24 hours
            _userService.Save(membership);

            //Send Password Reset Mail
            _userService.SendPasswordResetMail(membership.UserId, membership.UserProfile.UserName, membership.PasswordVerificationToken);

            return true;
        }

    }
}