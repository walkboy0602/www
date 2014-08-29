using App.Core.Data;
using App.Core.ViewModel;
using App.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using System.Web.Caching;

namespace App.Core.Services
{
    public interface IRegionService
    {
        IEnumerable<RegionZone> Get();
        IList<SelectListItem> GetRegionZone();
        RegionZone Find(int id);

        IEnumerable<RegionZone> GetRegionByParentId(int parentId);
        RegionZone GetParentRegionZoneByName(string name);
    }

    public class RegionService : IRegionService
    {
        private AdsDBEntities db;

        private string cacheKey = "regionZones";

        public RegionService()
        {
            this.db = new AdsDBEntities();
        }

        IEnumerable<RegionZone> IRegionService.Get()
        {
            IEnumerable<RegionZone> regionZones = null;

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                regionZones = (IEnumerable<RegionZone>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                regionZones = this.db.RegionZones
                                .Where(r => r.isActive == true);
                HttpRuntime.Cache.Insert(cacheKey, regionZones, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }

            return regionZones;
        }

        RegionZone IRegionService.Find(int id)
        {
            var regionZone = (this as IRegionService).Get()
                                .Where(r => r.id == id);

            return regionZone.FirstOrDefault();
        }

        IEnumerable<RegionZone> IRegionService.GetRegionByParentId(int parentId)
        {
            var regionZones = (this as IRegionService).Get()
                                .Where(r => r.ParentId == parentId);

            return regionZones;
        }

        RegionZone IRegionService.GetParentRegionZoneByName(string name)
        {
            var regionZone = (this as IRegionService).Get()
                                .Where(r => r.ParentId == null)
                                .Where(r => r.Name.ToLower() == name.ToLower()).FirstOrDefault();

            return regionZone;
        }

        IList<SelectListItem> IRegionService.GetRegionZone()
        {
            var list = Grouping(db.RegionZones)
                              .OfType<RegionZoneGroup>()
                //.Where(x => x.Value.isActive == true)
                              .SelectMany(x => GetNodeAndChildren(x))
                              .Select(t => new SelectListItem
                              {
                                  Text = t.Value.Name,
                                  Value = t.Value.id.ToString()
                              }).ToList();

            return list;
        }

        IEnumerable<RegionZoneGroup> GetNodeAndChildren(RegionZoneGroup node, bool checkIsActive = true)
        {
            return new[] { node }.Concat(node.Children
                                            .OfType<RegionZoneGroup>()
                                            .WhereIf(checkIsActive, x => x.Value.isActive == true)
                                            .Select(i => new RegionZoneGroup
                                            {
                                                Children = i.Children,
                                                Value = new RegionZone
                                                {
                                                    id = i.Value.id,
                                                    ParentId = i.Value.ParentId,
                                                    //Name = "\u21B3\xA0" + i.Value.Name,
                                                    Name = "\xA0›\xA0" + i.Value.Name,
                                                    isActive = i.Value.isActive
                                                }
                                            })
                                            .SelectMany(x => GetNodeAndChildren(x)
                                                .Select(i => new RegionZoneGroup
                                                {
                                                    Children = i.Children,
                                                    Value = new RegionZone
                                                    {
                                                        id = i.Value.id,
                                                        ParentId = i.Value.ParentId,
                                                        Name = "\xA0\xA0" + i.Value.Name,
                                                        isActive = i.Value.isActive
                                                    }
                                                })
                                            ));
        }

        IEnumerable<RegionZoneGroup> Grouping(IEnumerable<RegionZone> allCategories)
        {
            var allNodes = allCategories.Select(team => new RegionZoneGroup() { Value = team })
                            .ToList();
            var lookup = allNodes.ToLookup(team => team.Value.ParentId);
            foreach (var node in allNodes)
                node.Children = lookup[node.Value.id];
            return lookup[null];
        }

    }
}
