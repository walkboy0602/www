//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App.Core.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Membership
    {
        public Membership()
        {
            this.UserRoles = new HashSet<UserRole>();
        }
    
        public int UserId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string EmailConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public Nullable<System.DateTime> LastPasswordFailureDate { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public Nullable<System.DateTime> PasswordChangedDate { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordVerificationToken { get; set; }
        public Nullable<System.DateTime> PasswordVerificationTokenExpirationDate { get; set; }
        public bool IsMobileConfirmed { get; set; }
        public Nullable<bool> IsIdentificationConfirmed { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsOnline { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
        public Nullable<System.DateTime> LastLockoutDate { get; set; }
        public string Comment { get; set; }
    
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
