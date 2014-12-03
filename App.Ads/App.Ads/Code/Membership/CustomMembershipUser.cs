﻿using System;
using System.Web.Security;
using App.Core.ViewModel;
using App.Core.Data;
using System.Collections.Generic;

namespace App.Ads.Code.Membership
{
    public class CustomMembershipUser: MembershipUser
    {
        public int UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string Mobile { get; set; }
        public string[] roles { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }

        public CustomMembershipUser(App.Core.Data.Membership user)
            : base("CustomMembershipProvider", 
                        user.UserProfile.LastName,
                        user.UserId,
                        user.UserProfile.UserName,
                        string.Empty,
                        user.Comment,
                        user.IsEmailConfirmed,
                        user.IsLockedOut,
                        user.CreateDate,
                        user.LastLoginDate ?? DateTime.MinValue,
                        user.LastLoginDate ?? DateTime.MinValue,
                        user.LastPasswordChangedDate ?? DateTime.MinValue,
                        user.LastLockoutDate ?? DateTime.MinValue)
        {
            UserId = user.UserId;
            FirstName = user.UserProfile.FirstName;
            LastName = user.UserProfile.LastName;
            IsEmailConfirmed = user.IsEmailConfirmed;
            UserRoles = user.UserRoles;
            Mobile = user.UserProfile.Mobile;
        }
    }
}