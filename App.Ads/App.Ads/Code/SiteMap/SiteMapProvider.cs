using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;
using App.Core.Data;
using App.Ads.Code.Helpers;
using App.Core.ViewModel;
using App.Core.Services;

namespace App.Ads.Code.SiteMap
{

    public class ListingDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var db = new AdsDBEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;

                foreach (var location in db.RegionZones)
                {
                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Title = location.Name;
                    //dynamicNode.Key = "Location_" + location.id;
                    dynamicNode.ParentKey = "Home";
                    yield return dynamicNode;
                }
            }
        }
    }

    public class CategoryDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var db = new AdsDBEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                foreach (var category in db.RefCategories)
                {
                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Title = category.DisplayName;
                    dynamicNode.Key = "Category_" + category.id;
                    dynamicNode.ParentKey = category.ParentID == null ? "Listing" : "Category_" + category.ParentID;

                    //if(!dynamicNode.RouteValues.ContainsKey("LocationText"))
                    //{
                    //    dynamicNode.RouteValues.Add("LocationText", "Malaysia");
                    //}

                    //dynamicNode.PreservedRouteParameters.Add("LocationText");

                    //if (dynamicNode.RouteValues.Values.Contains("LocationText"))
                    //{

                    //}
                    //else
                    //{
                    //    dynamicNode.RouteValues.Add("LocationText", "Malaysia");

                    //}

                    dynamicNode.RouteValues.Add("CategoryText", category.Name.ToSeoUrl());

                    yield return dynamicNode;
                }
            }
        }
    }

    public class AdDetailDynamicNodeProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var db = new AdsDBEntities())
            {
                db.Configuration.LazyLoadingEnabled = false;
                // Create a node for each category 
                var listings = db.Listings.Include("RefCategory").Where(l => l.Status != (int)XtEnum.ListingStatus.New);

                if (listings != null)
                {
                    foreach (var listing in listings)
                    {
                        DynamicNode dynamicNode = new DynamicNode();
                        dynamicNode.Title = listing.Title;
                        dynamicNode.ParentKey = "Category_" + listing.RefCategory.id;
                        dynamicNode.RouteValues.Add("id", listing.Id);
                        dynamicNode.RouteValues.Add("adTitle", listing.Title.ToSeoUrl());

                        yield return dynamicNode;
                    }
                }
            }
        }
    }
}