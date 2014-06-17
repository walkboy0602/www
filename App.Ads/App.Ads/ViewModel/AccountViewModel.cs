using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using App.Core.Utility;
using App.Ads.Models;
using App.Ads.Areas.Account.Models;

namespace App.Ads.ViewModel
{
    public class AccountEditViewModel : BaseModel
    {
        public ChangePassword ChangePassword { get; set; }

        public EditUserProfile Profile { get; set; }
    }
}