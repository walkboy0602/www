using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Data;

namespace App.Core.ViewModel
{
    public class DropDownViewModel
    {
        //public IList<SelectListItem> CategoryList { get; private set; }

        //public CategoryViewModel(IEnumerable<RefCategory> items, int? selected = null)
        //{
        //    this.CategoryList = GetCategories(items, null);
        //}

        public IList<SelectListItem> GetCategories(IEnumerable<RefCategory> items, int? selected = null)
        {
            var list = Grouping(items)
                                .OfType<CategoryGroup>()
                                .Where(x => x.Value.isActive == true)
                                .SelectMany(x => GetNodeAndChildren(x))
                                .Select(t => new SelectListItem
                                {
                                    Text = t.Value.Name,
                                    Value = t.Value.id.ToString(),
                                    Selected = t.Value.id == selected ? true : false
                                }).ToList();

            return list;
        }

        public IEnumerable<CategoryGroup> GetNodeAndChildren(CategoryGroup node, bool checkIsActive = true)
        {
            return new[] { node }.Concat(node.Children
                                            .OfType<CategoryGroup>()
                                            .WhereIf(checkIsActive, x => x.Value.isActive == true)
                                            .Select(i => new CategoryGroup
                                            {
                                                Children = i.Children,
                                                Value = new RefCategory
                                                {
                                                    id = i.Value.id,
                                                    ParentID = i.Value.ParentID,
                                                    //Name = "\u21B3\xA0" + i.Value.Name,
                                                    Name = "\xA0›\xA0" + i.Value.Name,
                                                    isActive = i.Value.isActive,
                                                    Description = i.Value.Description
                                                }
                                            })
                                            .SelectMany(x => GetNodeAndChildren(x)
                                                .Select(i => new CategoryGroup
                                                {
                                                    Children = i.Children,
                                                    Value = new RefCategory
                                                    {
                                                        id = i.Value.id,
                                                        ParentID = i.Value.ParentID,
                                                        Name = "\xA0\xA0" + i.Value.Name,
                                                        isActive = i.Value.isActive,
                                                        Description = i.Value.Description
                                                    }
                                                })
                                            ));
        }

        public IEnumerable<CategoryGroup> Grouping(IEnumerable<RefCategory> allCategories)
        {
            var allNodes = allCategories.Select(team => new CategoryGroup() { Value = team })
                            .ToList();
            var lookup = allNodes.ToLookup(team => team.Value.ParentID);
            foreach (var node in allNodes)
                node.Children = lookup[node.Value.id];
            return lookup[null];
        }


        public IList<SelectListItem> CurrencyList(string selected = null)
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "RM", Value = "RM" });
            list.Add(new SelectListItem { Text = "SGD", Value = "SGD", Selected = true});
            list.Add(new SelectListItem { Text = "USD", Value = "USD" });

            return list.Select(l => new SelectListItem
            {
                Selected = (l.Value == selected),
                Text = l.Text,
                Value = l.Value
            }).ToList();
        }

    }
}