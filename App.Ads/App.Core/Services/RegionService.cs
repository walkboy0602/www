using App.Core.Data;
using App.Core.ViewModel;
using App.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Core.Services
{
    public interface IRegionService
    {
        IList<SelectListItem> GetRegionZone(int? selected = null);
        RegionZone GetRegionZoneById(int? id);

    }

    public class RegionService : IRegionService
    {
        private AdsDBEntities db;

        public RegionService()
        {
            this.db = new AdsDBEntities();
        }

        RegionZone IRegionService.GetRegionZoneById(int? id)
        {
            var regionZone = from r in db.RegionZones
                             where r.id == id
                             select r;

            return regionZone.FirstOrDefault();
        }

        IList<SelectListItem> IRegionService.GetRegionZone(int? selected = null)
        {
            var list = Grouping(db.RegionZones)
                              .OfType<RegionZoneGroup>()
                              //.Where(x => x.Value.isActive == true)
                              .SelectMany(x => GetNodeAndChildren(x))
                              .Select(t => new SelectListItem
                              {
                                  Text = t.Value.Name,
                                  Value = t.Value.id.ToString(),
                                  Selected = t.Value.id == selected ? true : false
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
