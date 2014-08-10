using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcSiteMapProvider;

namespace App.Ads.Code.SiteMap
{
    public class CustomSiteMapProvider : DynamicNodeProviderBase
    {
        public override IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode node)
        {
            var nodes = new[] {
            new DynamicNode
            {
               Title = "Dynamic 1",
               Controller = "dynamic1",
               Key = "dynamic1",
               ParentKey = "SearchIndex",
               Action = "Index"
            },
            new DynamicNode
            {
               Title = "Dynamic 1.1",
               Controller = "dynamic1",
               Key = "dynamic1.1",
               ParentKey = "dynamic1",
               Action = "Index"
            },
            new DynamicNode
            {
               Title = "Dynamic 2",
               Controller = "dynamic2",
               Key = "dynamic2",
               ParentKey = "SearchIndex",
               Action = "Index"
            }
         };
            nodes[0].RouteValues.Add("id", 0);
            nodes[1].RouteValues.Add("id", 1);
            return nodes;
        }

        

        //public IList<SiteMapModel> GetSiteMapData(int count)
        //{
        //    return _posts.AsNoTracking().OrderByDescending(post => post.CreatedDate).Take(count).
        //                  Select(post => new SiteMapModel
        //                  {
        //                      Id = post.Id,
        //                      CreatedDate = post.CreatedDate,
        //                      ModifiedDate = post.ModifiedDate,
        //                      Title = post.Title
        //                  }).ToList();
        //}
    }
}