using App.Core;
using App.Core.Data;
using App.Core.Services;
using App.Ads.Code.Membership;
using App.Ads.Code.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Ads.Controllers
{
    [Authorize]
    public class ListingController : Controller
    {
        private readonly ICategoryService categoryService;
        private readonly IRegionService regionService;
        private readonly IListingService listingService;
        CustomIdentity identity;

        public ListingController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.regionService = DependencyResolver.Current.GetService<IRegionService>();
            this.listingService = DependencyResolver.Current.GetService<IListingService>();
        }

        // GET: Listing
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            identity = User.ToCustomPrincipal().CustomIdentity;
            int ListingId = listingService.GetNewListing(identity.UserId);

            if (ListingId == 0)
            {
                ListingId = listingService.CreateNew(identity.UserId);
            }

            return RedirectToAction("Save", new { id = ListingId });
        }

        // Get: Listing/5

        public ActionResult Save(int id)
        {
            identity = User.ToCustomPrincipal().CustomIdentity;
            Listing listing = listingService.GetListing(id, identity.UserId);

            if (listing == null)
            {
                return RedirectToAction("Index");
            }

            RebindForm();

            return View(listing);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(Listing listing)
        {
            identity = User.ToCustomPrincipal().CustomIdentity;
            
            if (ModelState.IsValid)
            {

                return RedirectToAction("index");
            }

            RebindForm();
            return View(listing);
        }


        #region Helper

        private void RebindForm()
        {
            ViewBag.Contacts = listingService.GetContactMethods();
            ViewBag.Categories = categoryService.GetCategories();
            ViewBag.RegionZones = regionService.GetRegionZone();
        }

        #endregion

    }
}