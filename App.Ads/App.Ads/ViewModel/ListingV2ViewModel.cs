using ExpressiveAnnotations.ConditionalAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Ads.ViewModel
{
    public class ListingV2ViewModel
    {
    }

    public class ListingCreateViewModel
    {
        public ListingGeneralInfo GeneralInfo { get; set; }
        public ListingPropertyDetail PropertyDetail { get; set; }
    }

    public class ListingGeneralInfo
    {
        [Required(ErrorMessage = "Please enter Title.")]
        [StringLength(50, ErrorMessage = "Max {0} characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Minimun {2} charaters, Max {1} characters.")]
        [AllowHtml]
        public string Description { get; set; }

        [RegularExpression(@"^\$?\d+(\.(\d{2}))?$", ErrorMessage = "e.g price format - 19.99")]
        [Range(0, 99999999.99, ErrorMessage = "Price cannot exceed 8 characters long.")]
        [Required]
        public decimal? Price { get; set; }

        public bool IsNegotiable { get; set; }

        public bool ContactMe { get; set; }

        [Required(ErrorMessage = "Please select Location.")]
        public int LocationId { get; set; }

        public Nullable<int> AreaId { get; set; }
    }

    public class ListingPropertyDetail
    {
        public int TypeId { get; set; }
        public int Bedroom { get; set; }
        public int Bathroom { get; set; }

        [Required(ErrorMessage = "Size is required")]
        public int Size { get; set; }
        public int OtherInfo { get; set; }
    }
}