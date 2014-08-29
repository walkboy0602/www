using App.Core.Data;
using App.Core.Models;
using App.Core.ViewModel;
using App.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
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
        RefCategory FindByName(string name);

        IEnumerable<RefCategory> Get();
        IEnumerable<RefCategory> GetByParentID(int? ParentID);
        List<int> GetSubCategory(int categoryId);

        //DropDown
        IList<SelectListItem> GetCategories();
        IList<SelectListItem> GetParentCategories();
        List<DropDownModel> GetCategoriesOptGroup();
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

        private string cacheKey = "refCategories";

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

        RefCategory ICategoryService.Find(int id)
        {
            return db.RefCategories.Find(id);
        }

        RefCategory ICategoryService.FindByName(string name)
        {
            IEnumerable<RefCategory> refCategories = (this as ICategoryService).Get().Where(r => r.Name.ToLower() == name.ToLower());

            return refCategories.FirstOrDefault();
        }

        IEnumerable<RefCategory> ICategoryService.Get()
        {
            IEnumerable<RefCategory> refCategories = null;

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                refCategories = (IEnumerable<RefCategory>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                refCategories = db.RefCategories.Where(x => x.isActive == true).ToList();
                HttpRuntime.Cache.Insert(cacheKey, refCategories, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
            }

            return refCategories;
        }

        /// <summary>
        /// Retrieve all sub category of the selected categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        List<int> ICategoryService.GetSubCategory(int categoryId)
        {
            var ids = (this as ICategoryService).Get()
                                .First(c => c.id == categoryId)
                                .AllSubcategories()
                                .Select(x => x.id)
                                .ToList();

            return ids;
        }

        IList<SelectListItem> ICategoryService.GetCategories()
        {
            var list = Grouping(db.RefCategories)
                               .OfType<CategoryGroup>()
                               .Where(x => x.Value.isActive == true)
                               .SelectMany(x => GetNodeAndChildren(x))
                               .Select(t => new SelectListItem
                               {
                                   Text = t.Value.Name,
                                   Value = t.Value.id.ToString(),
                                   Selected = false
                               }).ToList();

            return list;
        }

        IList<SelectListItem> ICategoryService.GetParentCategories()
        {
            var list = db.RefCategories
                        .Where(r => r.ParentID == null)
                        .Where(r => r.isActive == true)
                               .Select(t => new SelectListItem
                               {
                                   Text = t.Name,
                                   Value = t.id.ToString(),
                                   Selected = false
                               }).ToList();

            return list;
        }

        List<DropDownModel> ICategoryService.GetCategoriesOptGroup()
        {
            var list = (this as ICategoryService).Get()
                        .Where(r => r.ParentID != null)
                        .Where(r => r.isActive == true)
                        .Select(t => new DropDownModel
                        {
                            Id = t.id.ToString(),
                            Name = t.Name,
                            Category = t.ParentCategory.Name
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
                                                    Name = "\xA0\xA0" + i.Value.Name,
                                                    //Name = "\xA0›\xA0" + i.Value.Name,
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
                                                        //Name = "\xA0\xA0" + i.Value.Name,
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

        protected virtual void Dispose(bool disposing)
        {
            if (db != null)
            {
                db.Dispose();
            }
        }


    }
}
