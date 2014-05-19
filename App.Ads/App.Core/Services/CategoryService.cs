using App.Core.Data;
using App.Core.Models;
using App.Core.ViewModel;
using App.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;

namespace App.Core.Services
{
    public interface ICategoryService
    {
        // Add any custom business logic (methods) here
        // All methods in Service<TEntity> are ovverridable for any custom implementations
        void Create(RefCategory refCategory);
        void Save(RefCategory refCategory);
        RefCategory Find(int id);
        IEnumerable<RefCategory> GetByParentID(int? ParentID);
        string GetParentName(int? ParentID);

        IList<SelectListItem> GetCategories(int? selected = null);
    }

    // Add any custom business logic (methods) here
    // All methods in Service<TEntity> are ovverridable for any custom implementations
    // Can ovveride any of the Repository methods to add business logic in them
    // e.g.
    //public override void Delete(Customer entity)
    //{
    //    // Add business logic before or after deleting entity.
    //    base.Delete(entity);
    //}
    public class CategoryService : ICategoryService
    {
        private AdsDBEntities db;

        public CategoryService()
        {
            this.db = new AdsDBEntities();
        }

        void ICategoryService.Create(RefCategory refCategory)
        {
            db.RefCategories.Add(refCategory);
            db.SaveChanges();
        }

        void ICategoryService.Save(RefCategory refCategory)
        {
            db.Entry(refCategory).State = EntityState.Modified;
            db.SaveChanges();
        }

        IEnumerable<RefCategory> ICategoryService.GetByParentID(int? ParentID)
        {
            return db.RefCategories.Where(x => x.ParentID == ParentID).ToList();
        }

        string ICategoryService.GetParentName(int? ParentID)
        {
            return db.RefCategories.Where(x => x.id == ParentID).Select(x => x.Name).SingleOrDefault();
        }

        RefCategory ICategoryService.Find(int id)
        {
            return db.RefCategories.Find(id);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        IList<SelectListItem> ICategoryService.GetCategories(int? selected = null)
        {
            var list = Grouping(db.RefCategories)
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

        IEnumerable<CategoryGroup> GetNodeAndChildren(CategoryGroup node, bool checkIsActive = true)
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

        IEnumerable<CategoryGroup> Grouping(IEnumerable<RefCategory> allCategories)
        {
            var allNodes = allCategories.Select(team => new CategoryGroup() { Value = team })
                            .ToList();
            var lookup = allNodes.ToLookup(team => team.Value.ParentID);
            foreach (var node in allNodes)
                node.Children = lookup[node.Value.id];
            return lookup[null];
        }

    }
}
