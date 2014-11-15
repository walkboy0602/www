using App.Core;
using App.Core.Data;
using App.Core.Services;
using App.Core.ViewModel;
using App.Core.Utility;
using App.Ads.Code.Membership;
using App.Ads.Code.Filters;
using App.Ads.Code.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using AutoMapper;

namespace App.Ads.Controllers
{
    [CustomAuthorize]
    public class ListingController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IRegionService regionService;
        private readonly IListingService listingService;
        private readonly IFeatureService featureService;
        private readonly IReferenceService referenceService;

        public ListingController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.regionService = DependencyResolver.Current.GetService<IRegionService>();
            this.listingService = DependencyResolver.Current.GetService<IListingService>();
            this.featureService = DependencyResolver.Current.GetService<IFeatureService>();
            this.referenceService = DependencyResolver.Current.GetService<ReferenceService>();
        }

        #region GET

        [HttpGet]
        public ActionResult Manage()
        {
            List<int> filterStatus = new List<int>{
                (int)XtEnum.ListingStatus.New,
                (int)XtEnum.ListingStatus.Deleted
            };

            var listings = listingService.GetAllByUserId(CurrentUser.CustomIdentity.UserId)
                            .Where(l => !filterStatus.Contains(l.Status))
                            .OrderByDescending(l => l.LastUpdate)
                            .ToList();

            listings.ForEach(x =>
            {
                x.Status = x.PostingEndDate < DateTime.Now ? (int)XtEnum.ListingStatus.Expired : x.Status;
            });

            var model = Mapper.Map<List<Listing>, List<DisplayListingViewModel>>(listings);

            return View(model);
        }


        [HttpGet]
        public ActionResult Create()
        {
            int ListingId = listingService.GetNewListing(CurrentUser.CustomIdentity.UserId);

            if (ListingId == 0)
            {
                ListingId = listingService.CreateNew(CurrentUser.CustomIdentity.UserId);
            }

            return RedirectToAction("Save", new { id = ListingId });
        }


        // Get: Listing/Save/5
        [HttpGet]
        public ActionResult Save(int id)
        {
            var listing = listingService.GetListingById(id, CurrentUser.CustomIdentity.UserId);

            if (listing == null)
            {
                return RedirectToAction("Manage");
            }

            EditListingViewModel model = Mapper.Map<Listing, EditListingViewModel>(listing);

            ///TODO: Better way to do this?
            //foreach (var item in model.ListingDealMethods)
            //{
            //    switch (item.DealMethodId)
            //    {
            //        case (int)XtEnum.DealMethod.COD:
            //            model.COD = true;
            //            model.CODText = item.Description;
            //            break;

            //        case (int)XtEnum.DealMethod.Postage:
            //            model.Postage = true;
            //            model.PostageText = item.Description;
            //            break;

            //        case (int)XtEnum.DealMethod.OnlineBanking:
            //            model.OnlineBanking = true;
            //            model.OnlineBankingText = item.Description;
            //            break;
            //    }
            //}

            RebindForm(model.LocationId);

            ViewBag.id = model.id;

            return View(model);
        }

        // Get: Listing/Publish/5
        [HttpGet]
        public ActionResult Publish(int id)
        {
            var listing = listingService.GetListingById(id, CurrentUser.CustomIdentity.UserId);

            //Redirect if TNC was accepted
            if (listing == null || listing.IsTNCAccept)
            {
                return RedirectToAction("Manage");
            }

            ViewBag.ListingId = id;
            RebindPublishForm();
            return View();
        }

        // GET : Listing/Image/5
        [HttpGet]
        public ActionResult Image(int ListingId)
        {
            ViewBag.ListingId = ListingId;

            var listing = listingService.GetListingById(ListingId, CurrentUser.CustomIdentity.UserId);

            if (listing == null)
            {
                return Content("Bad Request");
            }

            return PartialView("_gallery");
        }

        // Get: Listing/Complete
        [HttpGet]
        public ActionResult Complete()
        {
            if (TempData["Message"] == null)
            {
                return RedirectToAction("Manage");
            }
            return View();
        }

        #endregion

        #region POST

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
                //if (model.COD)
                //{
                //    listing.ListingDealMethods.Add(new ListingDealMethod
                //    {
                //        ListingId = model.id,
                //        DealMethodId = (int)XtEnum.DealMethod.COD,
                //        Description = model.CODText
                //    });
                //}

                //if (model.Postage)
                //{
                //    listing.ListingDealMethods.Add(new ListingDealMethod
                //    {
                //        ListingId = model.id,
                //        DealMethodId = (int)XtEnum.DealMethod.Postage,
                //        Description = model.PostageText
                //    });
                //}

                //if (model.OnlineBanking)
                //{
                //    listing.ListingDealMethods.Add(new ListingDealMethod
                //    {
                //        ListingId = model.id,
                //        DealMethodId = (int)XtEnum.DealMethod.OnlineBanking,
                //        Description = model.OnlineBankingText
                //    });
                //}

                listing.LastUpdate = DateTime.Now;
                listing.LastActionBy = CurrentUser.CustomIdentity.UserId;

                if (!listing.IsTNCAccept)
                {
                    listing.Status = (int)XtEnum.ListingStatus.Draft;
                    listing.IsComplete = true;
                    listingService.Save(listing);
                    return RedirectToAction("Publish", new { id = listing.Id });
                }
                else
                {
                    listing.Status = (int)XtEnum.ListingStatus.Pending;
                    listingService.Save(listing);

                    TempData["ListingDetail"] = listing;
                    TempData["Message"] = "You have successfully edit your listing, your listing will be temporary unavailable until reviewed by our team.";
                    return RedirectToAction("Complete");
                }

            }

            RebindForm(model.LocationId);
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
                //DateTime? featureEndDate = listing.ListingPurchaseLogs
                //                                .Where(l => l.EndDate.HasValue && l.EndDate >= DateTime.Now)
                //                                .Select(l => l.EndDate).FirstOrDefault();

                //if (featureEndDate != null && featureEndDate != DateTime.MinValue)
                //{
                //    ModelState.AddModelError("FeatureExists", "This listing already featured till " + featureEndDate);
                //    return View(model);
                //}

                ListingFeatureType featureType = featureService.Get(model.FeatureCode).FirstOrDefault();

                if (featureType == null)
                {
                    ModelState.AddModelError("Bad Request", "Bad Request - Invalid Listing Type.");
                    return View(model);
                }

                ListingPurchaseLog purchaseLog = new ListingPurchaseLog
                {
                    ListingId = listing.Id,
                    ListingTypeCode = model.FeatureCode,
                    PurchaseDate = DateTime.Now,
                };

                featureService.AddPurchaseLog(purchaseLog);

            }

            //TODO: Temp HardCode 60 days
            int duration = 60;

            // Update Listing
            listing.IsTNCAccept = true;
            listing.Status = (int)XtEnum.ListingStatus.Pending;
            listing.LastUpdate = DateTime.Now;
            listing.LastActionBy = CurrentUser.CustomIdentity.UserId;
            listing.Duration = duration;

            listingService.Save(listing);

            TempData["Message"] = "Thank you for posting your listing with us, your listing is currently being reviewed by us and will be available within 24 hours.";
            return RedirectToAction("Complete");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete (int id)
        {
            Listing listing = listingService.GetListingById(id, CurrentUser.CustomIdentity.UserId);

            if (listing == null)
            {
                return HttpNotFound();
            }

            listing.Status = (int)XtEnum.ListingStatus.Deleted;
            listing.LastAction = "Delete";
            listing.LastActionBy = CurrentUser.CustomIdentity.UserId;

            listingService.Save(listing);

            TempData["Message"] = "Your ad ''" + listing.Title + "'' has been successfully removed.";
            return RedirectToAction("Manage");
        }


        #endregion

        #region Helper

        private void RebindForm(int locationId = 0)
        {
            ViewBag.Contacts = listingService.GetContactMethods();
            //ViewBag.Categories = categoryService.GetCategories();

            ViewBag.LocationList = regionService.Get()
                                    .Where(r => r.ParentId == null)
                                    .OrderBy(r => r.Sort)
                                    .Select(r => new SelectListItem
                                    {
                                        Text = r.Name,
                                        Value = r.id.ToString()
                                    });

            ViewBag.ConditionList = referenceService.GetByType("CONDITION").ToList();

            ViewBag.AreaList = regionService.GetRegionByParentId(locationId)
                                    .OrderBy(r => r.Sort)
                                    .Select(r => new SelectListItem
                                    {
                                        Text = r.Name,
                                        Value = r.id.ToString()
                                    });

            ViewBag.ListingTypeList = referenceService.GetByType("LT").ToList();

            ViewBag.CategoryList = new SelectList(categoryService.GetCategoriesOptGroup(), "Id", "Name", "Category", 1);
        }

        private void RebindPublishForm()
        {
            ViewBag.DurationList = featureService.GetByGroupCode("Basic").OrderBy(o => o.sort)
                                            .Select
                                            (i => new SelectListItem()
                                            {
                                                Text = i.Name + " (RM " + i.Fees + ")",
                                                Value = i.Code
                                            });

            ViewBag.FeatureList = featureService.GetByGroupCode("FD");
        }

        #endregion

    }
}