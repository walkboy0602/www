using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;
using App.Core.Models;
using App.Core.ViewModel;
using App.Ads.Code.Helpers;

namespace App.Ads.ViewModel
{
    public class ListingDetailViewModel : ImageModel
    {
        public int id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public Nullable<decimal> Price { get; set; }

        public bool IsNegotiable { get; set; }

        public bool ContactMe { get; set; }

        public string ContactMethod { get; set; }

        public string ConditionCode { get; set; }

        public Nullable<int> AreaId { get; set; }

        public Nullable<int> LocationId { get; set; }

        public Nullable<int> CategoryId { get; set; }

        public DateTime PostedDate { get; set; }

        public string PostedBy { get; set; }

        public int Status { get; set; }

        public int LocationParentId { get; set; }

        public string ParentLocation { get; set; }

        public string CoverImage { get; set; }

        public virtual ICollection<AdImageViewModel> ListingImages { get; set; }

        public virtual RegionZone Location { get; set; }

        public virtual RegionZone Area { get; set; }

        public virtual RefCategory RefCategory { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public string Action { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }

        public string Place { get; set; }

    }

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

    public class AdImageViewModel
    {
        public int id { get; set; }
        public int ListingId { get; set; }
        public string FileName { get; set; }
        public bool IsCover { get; set; }
        public Nullable<byte> Sort { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Src { get; set; }
        public string Thumnbnail { get; set; }
        public string Description { get; set; }
    }

}
