using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;
using System.ComponentModel.DataAnnotations;

namespace App.Core.Data
{
    [MetadataType(typeof(ListingModel))]
    public partial class Listing
    {

    }

    public class ListingModel
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Keywords { get; set; }

        public Nullable<decimal> Price { get; set; }

        public bool IsNegotiable { get; set; }

        public Nullable<int> ContactMethod { get; set; }

        public Nullable<int> LocationId { get; set; }

        public int Status { get; set; }

        public Nullable<int> CreateBy { get; set; }

        public Nullable<System.DateTime> CreateDate { get; set; }

        public Nullable<System.DateTime> LastUpdate { get; set; }

        public virtual RefCategory RefCategory { get; set; }

        public virtual RegionZone RegionZone { get; set; }
    }
}
