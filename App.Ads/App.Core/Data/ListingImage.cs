//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace App.Core.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class ListingImage
    {
        public int id { get; set; }
        public int ListingId { get; set; }
        public string FileName { get; set; }
        public bool IsCover { get; set; }
        public Nullable<byte> Sort { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Src { get; set; }
        public string Description { get; set; }
    
        public virtual Listing Listing { get; set; }
    }
}
