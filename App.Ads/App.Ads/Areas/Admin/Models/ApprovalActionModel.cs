using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace App.Ads.Areas.Admin.Models
{
    public class ActionModel
    {
        public int ListingId { get; set; }

        public ApproveModel ApproveModel { get; set; }
        public RejectModel RejectModel { get; set; }
    }

    public class ApproveModel
    {
        [Required]
        public string SuccessCode { get; set; }
    }

    public class RejectModel
    {
        [Required(ErrorMessage="Select a Reason.")]
        public string RejectCode { get; set; }
    }
}