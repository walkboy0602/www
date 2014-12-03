using System;
using System.Security.Principal;
using System.Web.Security;
using System.Collections.Generic;
using App.Core.Data;

namespace App.Ads.Code.Membership
{
    public class CustomIdentity : IIdentity
    {

        #region Properties

        public IIdentity Identity { get; set; }

        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool isEmailConfirmed { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        #endregion

        #region Implementation of IIdentity

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <returns>
        /// The name of the user on whose behalf the code is running.
        /// </returns>
        public string Name
        {
            get { return Identity.Name; }
        }

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <returns>
        /// The type of authentication used to identify the user.
        /// </returns>
        public string AuthenticationType
        {
            get { return Identity.AuthenticationType; }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <returns>
        /// true if the user was authenticated; otherwise, false.
        /// </returns>
        public bool IsAuthenticated { get { return Identity.IsAuthenticated; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Save User Profile into Identity
        /// </summary>
        /// <param name="identity"></param>
        public CustomIdentity(IIdentity identity)
        {
            Identity = identity;

            var customMembershipUser = (CustomMembershipUser) System.Web.Security.Membership.GetUser(identity.Name);
            if (customMembershipUser != null)
            {
                UserId = customMembershipUser.UserId;
                FirstName = customMembershipUser.FirstName;
                LastName = customMembershipUser.LastName;
                Email = customMembershipUser.Email;
                isEmailConfirmed = customMembershipUser.IsEmailConfirmed;
                UserRoles = customMembershipUser.UserRoles;
                Mobile = customMembershipUser.Mobile;
            }
        }

        #endregion
    }
}