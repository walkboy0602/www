using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Core.Data;

namespace App.Ads.Controllers
{
    public class TestController : Controller
    {
        private AdsDBEntities db = new AdsDBEntities();

        // GET: /Test/
        public ActionResult Index()
        {
            var listings = db.Listings.Include(l => l.RefCategory).Include(l => l.RegionZone);
            return View(listings.ToList());
        }

        // GET: /Test/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listing listing = db.Listings.Find(id);
            if (listing == null)
            {
                return HttpNotFound();
            }
            return View(listing);
        }

        // GET: /Test/Create
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.RefCategories, "id", "Name");
            ViewBag.LocationId = new SelectList(db.RegionZones, "id", "Name");
            return View();
        }

        // POST: /Test/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="id,CategoryId,Title,Description,Keywords,Price,IsNegotiable,ContactMethod,LocationId,Status,CreateBy,CreateDate,LastUpdate")] Listing listing)
        {
            if (ModelState.IsValid)
            {
                db.Listings.Add(listing);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.RefCategories, "id", "Name", listing.CategoryId);
            ViewBag.LocationId = new SelectList(db.RegionZones, "id", "Name", listing.LocationId);
            return View(listing);
        }

        // GET: /Test/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listing listing = db.Listings.Find(id);
            if (listing == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.RefCategories, "id", "Name", listing.CategoryId);
            ViewBag.LocationId = new SelectList(db.RegionZones, "id", "Name", listing.LocationId);
            return View(listing);
        }

        // POST: /Test/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="id,CategoryId,Title,Description,Keywords,Price,IsNegotiable,ContactMethod,LocationId,Status,CreateBy,CreateDate,LastUpdate")] Listing listing)
        {
            if (ModelState.IsValid)
            {
                db.Entry(listing).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.RefCategories, "id", "Name", listing.CategoryId);
            ViewBag.LocationId = new SelectList(db.RegionZones, "id", "Name", listing.LocationId);
            return View(listing);
        }

        // GET: /Test/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listing listing = db.Listings.Find(id);
            if (listing == null)
            {
                return HttpNotFound();
            }
            return View(listing);
        }

        // POST: /Test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Listing listing = db.Listings.Find(id);
            db.Listings.Remove(listing);
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
