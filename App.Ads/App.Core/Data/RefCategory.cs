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
    
    public partial class RefCategory
    {
        public RefCategory()
        {
            this.SubCategories = new HashSet<RefCategory>();
            this.Listings = new HashSet<Listing>();
        }
    
        public int id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> Sort { get; set; }
        public Nullable<bool> isActive { get; set; }
        public string FaIcon { get; set; }
        public string DisplayName { get; set; }
        public string ListType { get; set; }
    
        public virtual ICollection<RefCategory> SubCategories { get; set; }
        public virtual RefCategory ParentCategory { get; set; }
        public virtual ICollection<Listing> Listings { get; set; }
    }
}
