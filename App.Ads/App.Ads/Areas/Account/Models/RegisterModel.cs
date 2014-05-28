using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using App.Core.Utility;

namespace App.Ads.Areas.Account.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Email")]
        [ValidEmailAddress(ErrorMessage = "Incorrect e-mail.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Display(Name = "Confirm Password")]
        [CompareAttribute("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Salutation { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Mobile Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0])([1])([0-9])\-[0-9]{7,8}", ErrorMessage = "Invalid phone number. It should be in the format of 012-3456789.")]
        public string Mobile { get; set; }
    }
}