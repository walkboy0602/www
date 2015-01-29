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
using App.Ads.Code.BO;

namespace App.Ads.Controllers
{
    [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
    public class CategoryController : Controller
    {
        private AdsDBEntities db = new AdsDBEntities();
        private readonly ICategoryService categoryService;
        private readonly IReferenceService referenceService;
        private readonly ICommonBO commonBO;

        public CategoryController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.referenceService = DependencyResolver.Current.GetService<ReferenceService>();
            this.commonBO = DependencyResolver.Current.GetService<ICommonBO>();
        }

        // GET: RefCategories
        public ActionResult Index(int? parentId)
        {
            return View(db.RefCategories.Where(c => c.ParentID == parentId).OrderBy(o => o.Sort).ToList());
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
            ViewBag.ListingTypeList = referenceService.GetByType("LT")
                                         .Select(r => new SelectListItem()
                                         {
                                             Text = r.Name,
                                             Value = r.Code
                                         });
            ViewBag.TemplateTypeList = commonBO.GetSelectListByRefType("TEMPLATE");
            ViewBag.Categories = categoryService.GetCategories();
            return View();
        }

        // POST: RefCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RefCategory refCategory)
        {
            if (ModelState.IsValid)
            {
                refCategory.isActive = true;
                db.RefCategories.Add(refCategory);
                db.SaveChanges();
                return RedirectToAction("Index", new { parentId = refCategory.ParentID });
            }
            ViewBag.ListingTypeList = referenceService.GetByType("LT")
                                       .Select(r => new SelectListItem()
                                       {
                                           Text = r.Name,
                                           Value = r.Code,
                                           Selected = r.Code == refCategory.ListType
                                       });
            ViewBag.TemplateTypeList = commonBO.GetSelectListByRefType("TEMPLATE", refCategory.TemplateType);
            ViewBag.Categories = categoryService.GetCategories();
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
            ViewBag.ListingTypeList = referenceService.GetByType("LT")
                                        .Select(r => new SelectListItem()
                                        {
                                            Text = r.Name,
                                            Value = r.Code,
                                            Selected = r.Code == refCategory.ListType
                                        });

            ViewBag.TemplateTypeList = commonBO.GetSelectListByRefType("TEMPLATE", refCategory.TemplateType);

            ViewBag.Categories = categoryService.GetCategories();
            return View(refCategory);
        }

        // POST: RefCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RefCategory refCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(refCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", new { parentId = refCategory.ParentID });
            }
            ViewBag.ListingTypeList = referenceService.GetByType("LT")
                                        .Select(r => new SelectListItem()
                                        {
                                            Text = r.Name,
                                            Value = r.Code,
                                             Selected = r.Code == refCategory.ListType
                                        });
            ViewBag.TemplateTypeList = commonBO.GetSelectListByRefType("TEMPLATE", refCategory.TemplateType);
            ViewBag.Categories = categoryService.GetCategories();

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
