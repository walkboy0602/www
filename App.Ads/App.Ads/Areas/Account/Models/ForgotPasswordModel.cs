using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using App.Core.Utility;
using App.Ads.Models;

namespace App.Ads.Areas.Account.Models
{
    public class ForgotPasswordModel : BaseModel
    {
        [Required]
        [Display(Name = "Email")]
        [ValidEmailAddress(ErrorMessage = "Invalid e-mail.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}