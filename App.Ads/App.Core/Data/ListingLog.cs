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
    
    public partial class ListingLog
    {
        public int id { get; set; }
        public Nullable<int> ListingId { get; set; }
        public string Action { get; set; }
        public Nullable<int> ActionBy { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public Nullable<int> Order { get; set; }
    
        public virtual Listing Listing { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
