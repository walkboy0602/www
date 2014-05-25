﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;

namespace App.Core.ViewModel
{
    public class AdDetailViewModel
    {
        public int id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get; set; }

        public Nullable<decimal> Price { get; set; }

        public bool IsNegotiable { get; set; }

        public Nullable<int> ContactMethod { get; set; }

        public Nullable<int> LocationId { get; set; }

        public string PostedBy { get; set; }

        public int Status { get; set; }

        public string Location { get; set; }

        public int LocationParentId { get; set; }

        public string ParentLocation { get; set; }

        public string CoverImage { get; set; }

        public virtual ICollection<ListingImage> ListingImages { get; set; }

        //public virtual RefCategory RefCategory { get; set; }
        //public virtual RegionZone RegionZone { get; set; }
        //public virtual ICollection<ListingDealMethod> ListingDealMethods { get; set; }
        //public virtual ICollection<ListingImage> ListingImages { get; set; }
    }

}
