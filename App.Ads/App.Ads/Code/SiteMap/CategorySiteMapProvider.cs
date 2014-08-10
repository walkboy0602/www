using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;
using App.Core.Data;

namespace App.Ads.Code.SiteMap
{
    public class CategorySiteMapProvider: DynamicNodeProviderBase
    {
        private AdsDBEntities context = new AdsDBEntities();

        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var db = new AdsDBEntities())
            {
                // Create a node for each album 
                foreach (var category in db.RefCategories)
                {
                    string categoryKey = category.id.ToString();

                    DynamicNode dynamicNode = new DynamicNode();
                    dynamicNode.Key = categoryKey;
                    dynamicNode.Title = category.Name;
                    dynamicNode.Controller = "People";
                    dynamicNode.Action = "Details";
                    dynamicNode.ParentKey = category.ParentID == null ? "SearchIndex" : category.ParentID.ToString(); // Attach to a node that is defined somewhere else
                    dynamicNode.RouteValues.Add("id", category.id.ToString());

                    yield return dynamicNode;
                }
            }
        }
    }

    public class AdDetailSiteMapProvider: DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            using (var db = new AdsDBEntities())
            {
                // Create a node for each album 
                foreach (var category in db.RefCategories.Where(x => x.Name == node.Key))
                {
                    string categoryKey = "Category_" + category.id.ToString();

                    DynamicNode staticNode = new DynamicNode();
                    staticNode.Title = category.Name + " Details";
                    staticNode.Key = category.id.ToString();
                    staticNode.ParentKey = categoryKey;
                    staticNode.Controller = "People";
                    staticNode.Action = "Details";
                    staticNode.RouteValues.Add("id", category.id.ToString());

                    yield return staticNode;
                }
            }
        }
    }
}