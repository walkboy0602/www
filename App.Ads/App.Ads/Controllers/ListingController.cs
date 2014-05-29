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
            identity = User.ToCustomPrincipal().CustomIdentity;

            var listings = listingService.GetListings(identity.UserId);

            var model = Mapper.Map<List<Listing>, List<DisplayListingViewModel>>(listings);

            int dayDiff;

            foreach (var item in model)
            {
                item.Title = string.IsNullOrEmpty(item.Title) ? "Untitled" : item.Title;
                item.Description = string.IsNullOrEmpty(item.Description) ? "" : Regex.Replace(item.Description, @"<[^>]*>", string.Empty);
                item.StatusText = Enum.GetName(typeof(XtEnum.ListingStatus), item.Status);
                item.StatusCss = GetStatusCss(item.Status);
                dayDiff = (DateTime.Now - item.CreateDate).Days;
                item.CreateDateText = dayDiff == 0 ? "Today" : dayDiff + " day ago";

                if (!string.IsNullOrEmpty(item.Description) && item.Description.Length > 167)
                {
                    item.Description = item.Description.Substring(0, 167) + "...";
                }

            }

            return View(model);
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
            var listing = listingService.GetListingById(id, identity.UserId);

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

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(EditListingViewModel model)
        {
            identity = User.ToCustomPrincipal().CustomIdentity;

            Listing listing = listingService.GetListingById(model.id, identity.UserId);

            if (listing == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
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

                listingService.Save(listing);

                return RedirectToAction("index");
            }

            RebindForm();
            return View(model);
        }

        public ActionResult Image(int id)
        {
            ViewBag.id = id;
            return PartialView("_gallery");
        }


        #region Helper

        private void RebindForm()
        {
            ViewBag.Contacts = listingService.GetContactMethods();
            ViewBag.Categories = categoryService.GetCategories();
            ViewBag.RegionZones = regionService.GetRegionZone();
        }

        private string GetStatusCss(int Status)
        {
            string cssClass = string.Empty;
            switch ((XtEnum.ListingStatus)Status)
            {
                case XtEnum.ListingStatus.Draft:
                case XtEnum.ListingStatus.New:
                    cssClass = "pull-right label label-default";
                    break;

                case XtEnum.ListingStatus.Expired:
                    cssClass = "pull-right label label-info";
                    break;

                case XtEnum.ListingStatus.Pending:
                    cssClass = "pull-right label label-warning";
                    break;

                case XtEnum.ListingStatus.Published:
                    cssClass = "pull-right label label-success";
                    break;

                case XtEnum.ListingStatus.Rejected:
                    cssClass = "pull-right label label-danger";
                    break;

                default:
                    cssClass = "pull-right label label-default";
                    break;
            }

            return cssClass;
        }

        #endregion

    }
}