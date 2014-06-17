using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using App.Core.Utility;
using App.Ads.Models;

namespace App.Ads.Areas.Account.Models
{
    public class EditUserProfile
    {
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