using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Core.Data;
using App.Core.Services;
using App.Ads.Code.Security;

namespace App.Ads.Controllers
{
    [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
    public class CategoryController : Controller
    {
        private AdsDBEntities db = new AdsDBEntities();
        private readonly ICategoryService categoryService;

        public CategoryController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
        }

        // GET: RefCategories
        public ActionResult Index()
        {
            return View(db.RefCategories.OrderBy(o => o.ParentID).ToList());
        }

        // GET: RefCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RefCategory refCategory = db.RefCategories.Find(id);
            if (refCategory == null)
            {
                return HttpNotFound();
            }
            return View(refCategory);
        }

        // GET: RefCategories/Create
        public ActionResult Create()
        {
            ViewBag.Categories = categoryService.GetParentCategories();
            return View();
        }

        // POST: RefCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,Name,ParentID,MetaDescription,MetaKeyword,Description,Sort,isActive")] RefCategory refCategory)
        {
            if (ModelState.IsValid)
            {
                db.RefCategories.Add(refCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = categoryService.GetParentCategories();
            return View(refCategory);
        }

        // GET: RefCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RefCategory refCategory = db.RefCategories.Find(id);
            if (refCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categories = categoryService.GetParentCategories();
            return View(refCategory);
        }

        // POST: RefCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,Name,ParentID,MetaDescription,MetaKeyword,Description,Sort,isActive")] RefCategory refCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(refCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = categoryService.GetParentCategories();
            return View(refCategory);
        }

        // GET: RefCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RefCategory refCategory = db.RefCategories.Find(id);
            if (refCategory == null)
            {
                return HttpNotFound();
            }
            return View(refCategory);
        }

        // POST: RefCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RefCategory refCategory = db.RefCategories.Find(id);
            db.RefCategories.Remove(refCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
