using System;
using System.Collections.Generic;
using App.Core.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Core.ViewModel
{

    public class DisplayListingViewModel : ImageModel
    {

        public int id { get; set; }

        public string Title { get; set; }

        public Nullable<decimal> Price { get; set; }

        public string ParentLocation { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime PostedDate { get; set; }

        public DateTime LastUpdate { get; set; }

        public DateTime PostingEndDate { get; set; }

        public int Status { get; set; }

        public virtual RefCategory RefCategory { get; set; }
        public virtual RegionZone RegionZone { get; set; }
        public virtual ICollection<ListingImage> ListingImages { get; set; }

        //private string ReplaceHTML(string str)
        //{
        //    return Regex.Replace(str, @"<[^>]*>", string.Empty);
        //}

    }


}
