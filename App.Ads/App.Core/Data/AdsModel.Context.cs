﻿

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

    public virtual DbSet<Listing_DealMethod> Listing_DealMethod { get; set; }

    public virtual DbSet<Listing> Listings { get; set; }

}

}

