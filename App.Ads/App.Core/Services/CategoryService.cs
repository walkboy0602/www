using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Core.Data;
using App.Core.Models;
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
        private ShopDBEntities db = new ShopDBEntities();

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

    }
}
