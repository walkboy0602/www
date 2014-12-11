using App.Core.Data;
using App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Caching;
using System.Web;
using App.Core.ViewModel;

namespace App.Core.Services
{
    public interface IUserService
    {
        // UserProfiles 
        UserProfile GetUserProfile(string userName);
        UserProfile GetUserProfile(int userId);
        void Save(UserProfile userProfile);

        // Membership
        void Save(App.Core.Data.Membership membership, bool isAdd);
        App.Core.Data.Membership GetMembership(string userName);
        App.Core.Data.Membership GetMembership(int userId);
        App.Core.Data.Membership GetMembershipByConfirmToken(string token, bool withUserProfile);
        App.Core.Data.Membership GetMembershipByPasswordVerificationToken(int userId, string token);

        // UserRoles

        // Emails
        void SendAccountActivationMail(string email);
        void SendAccountActivationMail(App.Core.Data.Membership membership);
        void SendPasswordResetMail(int userId, string email, string token);

        // Helper to do 
        string GetHash(string text);

    }

    public class UserService : IUserService
    {
        private readonly IConfigService configService;
        private readonly IEmailService emailService;

        private AdsDBEntities db;

        public UserService(IConfigService configService, IEmailService emailService)
        {
            db = new AdsDBEntities();
            this.configService = configService;
            this.emailService = emailService;
        }

        UserProfile IUserService.GetUserProfile(string userName)
        {
            return this.db.UserProfiles.FirstOrDefault(x => x.UserName.Equals(userName));
        }

        UserProfile IUserService.GetUserProfile(int userId)
        {
            return this.db.UserProfiles.FirstOrDefault(x => x.UserId.Equals(userId));
        }

        void IUserService.Save(UserProfile userProfile)
        {
            if (userProfile.UserId == 0)
            {
                this.db.UserProfiles.Add(userProfile);
            }

            this.db.SaveChanges();
        }

        private void ClearCache(string UserName)
        {
            var cacheKey = string.Format("UserData_{0}", UserName);

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                HttpRuntime.Cache.Remove(cacheKey);
            }
        }

        void IUserService.Save(App.Core.Data.Membership membership, bool isAdd)
        {
            if (isAdd)
            {
                this.db.Memberships.Add(membership);
            }

            this.db.SaveChanges();

        }

        App.Core.Data.Membership IUserService.GetMembership(string userName)
        {
            App.Core.Data.Membership membership = null;

            var userProfile = db.UserProfiles.Where(x => x.UserName.Equals(userName)).FirstOrDefault();
       
            if (userProfile != null)
            {
                membership = db.Memberships.Where(x => x.UserId == userProfile.UserId).FirstOrDefault();
                //Force to reload db data else it will cached.
                db.Entry(userProfile).Reload(); 
                db.Entry(membership).Reload();
                membership.UserProfile = userProfile;
            }

            return membership;
        }

        App.Core.Data.Membership IUserService.GetMembership(int userId)
        {
            var membership = db.Memberships.Where(x => x.UserId == userId).FirstOrDefault();

            if (membership != null)
            {
                var userProfile = db.UserProfiles.Where(x => x.UserId == userId).FirstOrDefault();
                //Force to reload db data else it will cached.
                db.Entry(userProfile).Reload();
                db.Entry(membership).Reload();
                membership.UserProfile = userProfile;
            }

            return membership;
        }

        App.Core.Data.Membership IUserService.GetMembershipByConfirmToken(string token, bool withUserProfile)
        {
            var membership = this.db.Memberships.FirstOrDefault(x => x.EmailConfirmationToken.Equals(token.ToLower()));
            if (membership != null && withUserProfile)
            {
                membership.UserProfile = this.db.UserProfiles.First(x => x.UserId == membership.UserId);
            }
            return membership;
        }

        App.Core.Data.Membership IUserService.GetMembershipByPasswordVerificationToken(int userId, string token)
        {
            var membership = this.db.Memberships
                                .Where(x => x.UserId == userId && x.PasswordVerificationToken.Equals(token.ToLower())).FirstOrDefault();
            return membership;
        }

        void IUserService.SendAccountActivationMail(string email)
        {
            var userProfile = (this as IUserService).GetUserProfile(email);
            if (userProfile == null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }

            var membership = (this as IUserService).GetMembership(userProfile.UserId);
            if (membership == null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }

            var viewData = new ViewDataDictionary { Model = userProfile };
            viewData.Add("Membership", membership);
            this.emailService.SendEmailWithTemplate(email, "Confirm your registration",
                "ConfirmRegistration",
                viewData, true
            );
        }

        void IUserService.SendAccountActivationMail(App.Core.Data.Membership membership)
        {
            var viewData = new ViewDataDictionary { Model = membership.UserProfile };
            viewData.Add("Membership", membership);
            this.emailService.SendEmailWithTemplate(membership.UserProfile.UserName, "Confirm your registration",
                "ConfirmRegistration",
                viewData, false
            );
            membership.LastConfirmEmailSent = DateTime.Now;
            (this as IUserService).Save(membership, false);
        }

        void IUserService.SendPasswordResetMail(int userId, string email, string token)
        {
            var userProfile = (this as IUserService).GetUserProfile(userId);
            if (userProfile == null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }

            var membership = (this as IUserService).GetMembership(userId);
            if (membership == null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.ProviderError);
            }

            var viewData = new ViewDataDictionary { Model = userProfile };
            viewData.Add("Membership", (App.Core.Data.Membership)membership);
            this.emailService.SendEmailWithTemplate(email, "Reset your password",
                  "ResetPassword",
                  viewData
              );
        }
        
        private string salt = "HJIO6589";
        string IUserService.GetHash(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(String.Concat(text, salt));
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            return hash;
        }

    }
}
