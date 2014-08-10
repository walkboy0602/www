﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AdsDBEntities : DbContext
    {
        public AdsDBEntities()
            : base("name=AdsDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Membership> Memberships { get; set; }
        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<RefCategory> RefCategories { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<RegionZone> RegionZones { get; set; }
        public virtual DbSet<ListingDealMethod> ListingDealMethods { get; set; }
        public virtual DbSet<ListingImage> ListingImages { get; set; }
        public virtual DbSet<Enquiry> Enquiries { get; set; }
        public virtual DbSet<z_RefCategory> z_RefCategory { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<ListingPurchaseLog> ListingPurchaseLogs { get; set; }
        public virtual DbSet<ListingType> ListingTypes { get; set; }
        public virtual DbSet<Listing> Listings { get; set; }
        public virtual DbSet<RefTable> RefTables { get; set; }
        public virtual DbSet<ListingLog> ListingLogs { get; set; }
    }
}
