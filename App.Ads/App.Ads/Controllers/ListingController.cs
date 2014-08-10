using App.Core;
using App.Core.Data;
using App.Core.Services;
using App.Core.ViewModel;
using App.Core.Utility;
using App.Ads.Code.Membership;
using App.Ads.Code.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using AutoMapper;

namespace App.Ads.Controllers
{
    [Authorize]
    public class ListingController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IRegionService regionService;
        private readonly IListingService listingService;
        private readonly IPaymentService paymentService;

        public ListingController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.regionService = DependencyResolver.Current.GetService<IRegionService>();
            this.listingService = DependencyResolver.Current.GetService<IListingService>();
            this.paymentService = DependencyResolver.Current.GetService<IPaymentService>();
        }

        #region GET

        public ActionResult Manage()
        {
            var listings = listingService.GetAllByUserId(CurrentUser.CustomIdentity.UserId).Where(l => l.Status != (int)XtEnum.ListingStatus.New).ToList();

            var model = Mapper.Map<List<Listing>, List<DisplayListingViewModel>>(listings);

            return View(model);
        }

        // Get: Listing/Save/5

        public ActionResult Save(int id)
        {
            var listing = listingService.GetListingById(id, CurrentUser.CustomIdentity.UserId);

            if (listing == null)
            {
                return RedirectToAction("Index");
            }

            EditListingViewModel model = Mapper.Map<Listing, EditListingViewModel>(listing);

            //TODO: Better way to do this?
            foreach (var item in model.ListingDealMethods)
            {
                switch (item.DealMethodId)
                {
                    case (int)XtEnum.DealMethod.COD:
                        model.COD = true;
                        model.CODText = item.Description;
                        break;

                    case (int)XtEnum.DealMethod.Postage:
                        model.Postage = true;
                        model.PostageText = item.Description;
                        break;

                    case (int)XtEnum.DealMethod.OnlineBanking:
                        model.OnlineBanking = true;
                        model.OnlineBankingText = item.Description;
                        break;
                }
            }

            RebindForm();

            ViewBag.id = model.id;

            return View(model);
        }

        // Get: Listing/Publish/5

        public ActionResult Publish(int id)
        {
            var listing = listingService.GetListingById(id, CurrentUser.CustomIdentity.UserId);

            if (listing == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.ListingId = id;
            RebindPublishForm();
            return View();
        }

        // GET : Listing/Image/5

        public ActionResult Image(int ListingId)
        {
            ViewBag.ListingId = ListingId;
            return PartialView("_gallery");
        }

        // Get: Listing/Complete

        public ActionResult Complete()
        {
            if (TempData["Messsage"] == null)
            {
                return RedirectToAction("Manage");
            }
            return View();
        }

        #endregion

        #region POST

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create()
        {
            int ListingId = listingService.GetNewListing(CurrentUser.CustomIdentity.UserId);

            if (ListingId == 0)
            {
                ListingId = listingService.CreateNew(CurrentUser.CustomIdentity.UserId);
            }

            return RedirectToAction("Save", new { id = ListingId });
        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(EditListingViewModel model)
        {
            Listing listing = listingService.GetListingById(model.id, CurrentUser.CustomIdentity.UserId);

            if (listing == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                ViewBag.ListingId = model.id;

                listing = Mapper.Map<EditListingViewModel, Listing>(model, listing);

                //TODO: Better way to do this?
                if (model.COD)
                {
                    listing.ListingDealMethods.Add(new ListingDealMethod
                    {
                        ListingId = model.id,
                        DealMethodId = (int)XtEnum.DealMethod.COD,
                        Description = model.CODText
                    });
                }

                if (model.Postage)
                {
                    listing.ListingDealMethods.Add(new ListingDealMethod
                    {
                        ListingId = model.id,
                        DealMethodId = (int)XtEnum.DealMethod.Postage,
                        Description = model.PostageText
                    });
                }

                if (model.OnlineBanking)
                {
                    listing.ListingDealMethods.Add(new ListingDealMethod
                    {
                        ListingId = model.id,
                        DealMethodId = (int)XtEnum.DealMethod.OnlineBanking,
                        Description = model.OnlineBankingText
                    });
                }

                listing.LastActionBy = CurrentUser.CustomIdentity.UserId;

                if (listing.PostingEndDate == null)
                {
                    listing.Status = (int)XtEnum.ListingStatus.Draft;
                    listing.IsComplete = true;
                    listingService.Save(listing);
                    return RedirectToAction("Publish", new { id = listing.id });
                }
                else
                {
                    listing.Status = (int)XtEnum.ListingStatus.Pending;
                    listingService.Save(listing);

                    TempData["ListingDetail"] = listing;
                    TempData["Message"] = "You have successfully edit your listing, your listing will be temporary unavailable until reviewed by us.";
                    return RedirectToAction("Complete");
                }

            }

            RebindForm();
            return View(model);

        }


        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Publish(int id, PublishListingViewModel model)
        {
            ViewBag.ListingId = id;
            RebindPublishForm();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var listing = listingService.GetListingById(id);

            if (listing == null)
            {
                ModelState.AddModelError("Bad Request", "Bad Request - listing not exists");
                return View(model);
            }

            if (!listing.IsComplete)
            {
                ModelState.AddModelError("Bad Request", "You haven't complete your listing, please go back to complete your listing.");
                return View(model);
            }

            if (model.FeatureCode != null)
            {
                DateTime featureEndDate = listing.ListingPurchaseLogs.Where
                                                (l => l.EndDate >= DateTime.Now).Select(l => l.EndDate).FirstOrDefault();

                if (featureEndDate != null && featureEndDate != DateTime.MinValue)
                {
                    ModelState.AddModelError("FeatureExists", "This listing already featured till " + featureEndDate);
                    return View(model);
                }

                ListingType listingType = paymentService.Get(model.FeatureCode).FirstOrDefault();

                if (listingType == null)
                {
                    ModelState.AddModelError("Bad Request", "Bad Request - Invalid Listing Type.");
                    return View(model);
                }

                // Insert Log
                if (listingType.Duration > 0)
                {
                    ListingPurchaseLog purchaseLog = new ListingPurchaseLog
                    {
                        ListingId = id,
                        ListingTypeCode = listingType.Code,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddDays((double)listingType.Duration)
                    };

                    paymentService.SavePurchaseLog(purchaseLog);
                }
            }

            // Validate Duration
            int duration = paymentService.Get(model.DurationCode).Select(f => f.Duration).FirstOrDefault();

            if (duration == 0)
            {
                ModelState.AddModelError("Bad Request", "Bad Request - Invalid Duration.");
                return View(model);
            }

            // Update Listing
            listing.IsTNCAccept = true;
            listing.Status = (int)XtEnum.ListingStatus.Pending;
            listing.PostedDate = DateTime.Now;
            listing.PostingEndDate = DateTime.Now.AddDays(duration);

            listingService.Save(listing);

            TempData["Message"] = "Thank you for posting your listing with us, your listing is currently being reviewed by us and will be available within 24 hours.";
            return RedirectToAction("Complete");
        }


        #endregion

        #region Helper

        private void RebindForm()
        {
            ViewBag.Contacts = listingService.GetContactMethods();
            ViewBag.Categories = categoryService.GetCategories();
            ViewBag.RegionZones = regionService.GetRegionZone();
        }

        private void RebindPublishForm()
        {
            ViewBag.DurationList = paymentService.GetByGroupCode("Basic").OrderBy(o => o.sort)
                                            .Select
                                            (i => new SelectListItem()
                                            {
                                                Text = i.Name + " (RM " + i.Fees + ")",
                                                Value = i.Code
                                            });

            ViewBag.FeatureList = paymentService.GetByGroupCode("FD");
        }

        #endregion

    }
}