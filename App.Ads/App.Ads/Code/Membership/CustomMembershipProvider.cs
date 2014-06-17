using App.Core;
using App.Core.Data;
using App.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web.Caching;

namespace App.Ads.Code.Membership
{
    public class CustomMembershipProvider : ExtendedMembershipProvider
    {
        //
        // Properties from web.config, default all to False
        //
        private int _cacheTimeoutInMinutes = 30;
        private string _ApplicationName;
        private bool _EnablePasswordReset;
        private bool _EnablePasswordRetrieval = false;
        private bool _RequiresQuestionAndAnswer = false;
        private bool _RequiresUniqueEmail = true;
        private int _MaxInvalidPasswordAttempts;
        private int _PasswordAttemptWindow;
        private int _MinRequiredPasswordLength;
        private int _MinRequiredNonalphanumericCharacters;
        private string _PasswordStrengthRegularExpression;
        private MembershipPasswordFormat _PasswordFormat = MembershipPasswordFormat.Hashed;

        private readonly IUserService usersService;

        public CustomMembershipProvider()
        {
            this.usersService = DependencyResolver.Current.GetService<IUserService>();
        }

        #region MembershipProvider

        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "CustomMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Custom Membership Provider");
            }

            base.Initialize(name, config);

            _ApplicationName = GetConfigValue(config["applicationName"], System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            _MaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            _PasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _MinRequiredNonalphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonalphanumericCharacters"], "1"));
            _MinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "6"));
            _EnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _PasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));

        }

        public override string ApplicationName
        {
            get { return _ApplicationName; }
            set { _ApplicationName = value; }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out System.Web.Security.MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override System.Web.Security.MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets information from the data source for a user, including UserProfile.
        /// Insert into UserData_{username} cache.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.Security.MembershipUser"/> object populated with the specified user's information from the data source.
        /// </returns>
        /// <param name="username">The name of the user to get information for. </param>
        /// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user. </param>
        public override System.Web.Security.MembershipUser GetUser(string username, bool userIsOnline)
        {
            var cacheKey = string.Format("UserData_{0}", username);

            if (HttpRuntime.Cache[cacheKey] != null)
                return (CustomMembershipUser)HttpRuntime.Cache[cacheKey];

            var user = this.usersService.GetMembership(username);

            var membershipUser = new CustomMembershipUser(user);

            //Store in cache
            HttpRuntime.Cache.Insert(cacheKey, membershipUser, null, DateTime.Now.AddMinutes(_cacheTimeoutInMinutes), Cache.NoSlidingExpiration);

            return membershipUser;
        }

        public override System.Web.Security.MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Web.Security.MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(System.Web.Security.MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {
            var userProfile = this.usersService.GetUserProfile(username);
            if (userProfile == null)
            {
                return false;
            }
            var membership = this.usersService.GetMembership(userProfile.UserId);
            if (membership == null)
            {
                return false;
            }
            //if (!membership.IsEmailConfirmed)
            //{
            //    return false;
            //}
            if (membership.PasswordSalt == this.usersService.GetHash(password))
            {
                return true;
            }
            // first once time we can validate through membership ConfirmationToken, 
            // to be logged in immediately after confirmation
            if (membership.EmailConfirmationToken != null)
            {
                if (membership.EmailConfirmationToken == password)
                {
                    membership.EmailConfirmationToken = null;
                    this.usersService.Save(membership);
                    return true;
                }
            }
            return false;
        }

        #endregion MembershipProvider

        #region ExtendedMembershipProvider

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            var membership = this.usersService.GetMembershipByConfirmToken(accountConfirmationToken, withUserProfile: false);
            if (membership == null)
            {
                throw new Exception("Activation code is incorrect.");
            }
            if (membership.IsEmailConfirmed)
            {
                throw new Exception("Your account is already activated.");
            }
            membership.IsEmailConfirmed = true;
            this.usersService.Save(membership);
            return true;
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override string CreateUserAndAccount(string userName/*email*/, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            userName = userName.Trim().ToLower();

            var userProfile = this.usersService.GetUserProfile(userName);
            if (userProfile != null)
            {
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateEmail);
            }

            var newUserProfile = new UserProfile
            {
                UserName = userName,
                FirstName = Convert.ToString(values.Where(v => v.Key == "FirstName").Select(v => v.Value).FirstOrDefault()),
                LastName = Convert.ToString(values.Where(v => v.Key == "LastName").Select(v => v.Value).FirstOrDefault()),
                Mobile = Convert.ToString(values.Where(v => v.Key == "Mobile").Select(v => v.Value).FirstOrDefault())
            };
            this.usersService.Save(newUserProfile);

            var membership = new App.Core.Data.Membership
            {
                UserId = newUserProfile.UserId,
                CreateDate = DateTime.Now,
                PasswordSalt = this.usersService.GetHash(password),
                IsEmailConfirmed = false,
                IsMobileConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString().ToLower()
            };
            this.usersService.Save(membership);

            return membership.EmailConfirmationToken;
        }

        public override bool DeleteAccount(string userName)
        {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            throw new NotImplementedException();
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromOAuth(string provider, string providerUserId)
        {
            //var oAuthMembership = this.usersService.GetOAuthMembership(provider, providerUserId);
            //if (oAuthMembership != null)
            //{
            //    return oAuthMembership.UserId;
            //}
            return -1;
        }

        public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
        {
            //var userProfile = this.usersService.GetUserProfile(userName);
            //if (userProfile == null)
            //{
            //    throw new Exception("User profile was not created.");
            //}
            //this.usersService.SaveOAuthMembership(provider, providerUserId, userProfile.UserId);
        }

        public override string GetUserNameFromId(int userId)
        {
            //var userProfile = this.usersService.GetUserProfile(userId);
            //if (userProfile != null)
            //{
            //    return userProfile.UserName;
            //}
            return null;
        }

        #endregion ExtendedMembershipProvider

        /*#region Helpers
        private const string salt = "HJIO6589";
        public static string GetHash(string text)
        {
            var buffer = Encoding.UTF8.GetBytes(String.Concat(text, salt));
            var cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            return hash;
        }
        #endregion Helpers*/

        #region Helpers

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        #endregion

    }
}