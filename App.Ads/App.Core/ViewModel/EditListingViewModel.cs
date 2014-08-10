using App.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.ConditionalAttributes;
using App.Core.Utility;
using System.Web.Mvc;

namespace App.Core.ViewModel
{
    public class EditListingViewModel
    {
        public int id { get; set; }

        public string Mode { get; set; }

        [Required(ErrorMessage="Please select a Category")]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Maximum {1} charaters.")]
        public string Title { get; set; }

        [Required]
        [AllowHtml]
        public string Description { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Maximum {1} charaters.")]
        public string Keywords { get; set; }

        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$", ErrorMessage = "e.g price format - 19.99")]
        [Range(0, 99999999.99, ErrorMessage = "Price cannot exceed 8 characters long.")]
        [Required]
        public decimal Price { get; set; }

        public bool IsNegotiable { get; set; }

        [Required]
        public int ContactMethod { get; set; }

        [Required(ErrorMessage="Please select Location.")]
        public int LocationId { get; set; }

        public int Status { get; set; }

        public Nullable<System.DateTime> LastUpdate { get; set; }

        public bool COD { get; set; }


        [StringLength(50, ErrorMessage = "Maximum {1} charaters.")]
        [RequiredIf(DependentProperty = "COD", TargetValue = true, ErrorMessage = "Please fill in the detail.")]
        public string CODText { get; set; }

        public bool Postage { get; set; }

        [StringLength(50, ErrorMessage = "Maximum {1} charaters.")]
        [RequiredIf(DependentProperty = "Postage", TargetValue = true, ErrorMessage = "Please fill in the detail.")]
        public string PostageText { get; set; }

        public bool OnlineBanking { get; set; }

        [StringLength(50, ErrorMessage = "Maximum {1} charaters.")]
        [RequiredIf(DependentProperty = "OnlineBanking", TargetValue = true, ErrorMessage = "Please fill in the detail.")]
        public string OnlineBankingText { get; set; }

        public string CreateDate { get; set; }

        public string LastAction = "Edit";

        public virtual ICollection<ListingDealMethod> ListingDealMethods { get; set; }


    }
}
