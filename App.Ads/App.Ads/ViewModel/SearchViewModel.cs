using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Core.Data;

namespace App.Ads.ViewModel
{
    public class SearchViewModel
    {
        public int id { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public Nullable<decimal> Price { get; set; }
        public bool IsNegotiable { get; set; }
        public Nullable<int> ContactMethod { get; set; }
        public Nullable<int> LocationId { get; set; }
        public int Status { get; set; }
        public Nullable<int> CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }

        public string ParentLocation { get; set; }

        public string CoverImage { get; set; }

        public virtual RefCategory RefCategory { get; set; }
        public virtual RegionZone RegionZone { get; set; }
        public virtual ICollection<ListingDealMethod> ListingDealMethods { get; set; }
        public virtual ICollection<ListingImage> ListingImages { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual ICollection<Enquiry> Enquiries { get; set; }

    }
}