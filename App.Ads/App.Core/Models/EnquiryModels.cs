using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using App.Core.Utility;
using App.Core.Data;

namespace App.Core.Models
{
    public class SendEnquiryModel
    {
        public int ListingId { get; set; }

        public string ListingTitle { get; set; }

        public UserProfile Recipient { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Email")]
        [ValidEmailAddress(ErrorMessage = "Incorrect e-mail.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Display(Name = "Mobile Phone")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^([0])([1])([0-9])\-[0-9]{7,8}", ErrorMessage = "Invalid phone number. It should be in the format of 012-3456789.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string Message { get; set; }

        public string Captcha { get; set; }
    }
}