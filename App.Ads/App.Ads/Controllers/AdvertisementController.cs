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
using App.Ads.ViewModel;
using AutoMapper;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using App.Ads.Code.Helpers;
using App.Core.ViewModel;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;

namespace App.Ads.Controllers
{
    public class AdvertisementController : BaseController
    {
        private AdsDBEntities db = new AdsDBEntities();

        private readonly ICategoryService categoryService;
        private readonly IRegionService regionService;
        private readonly IListingService listingService;

        public AdvertisementController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.regionService = DependencyResolver.Current.GetService<IRegionService>();
            this.listingService = DependencyResolver.Current.GetService<IListingService>();
        }

        // GET: Ad
        //[MvcSiteMapNode(Title = "Advertisement Details", PreservedRouteParameters = "id")]
        //[SiteMapTitle("CategoryId", Target = AttributeTarget.ParentNode)]
        //[MvcSiteMapNode(Title = "Ad Details", Key = "AdDetail", PreservedRouteParameters = "id, adTitle", ParentKey="Home")]
        [HttpGet]
        //[SiteMapCacheRelease()]
        public ActionResult Detail(int id, string adTitle, string a)
        {
            var listing = listingService.GetListingById(id);

            //Valiate listing status
            if (listing.Status != (int)XtEnum.ListingStatus.Live || listing.PostingEndDate <= DateTime.Now)
            {
                if (CurrentUser == null || listing.CreateBy != CurrentUser.CustomIdentity.UserId)
                {
                    return HttpNotFound();
                }
            }

            var model = Mapper.Map<Listing, ListingDetailViewModel>(listing);

            if (!string.IsNullOrEmpty(a))
            {
                model.Action = a;
            }

            string expectedTitle = model.Title.ToSeoUrl();
            string actualName = (adTitle ?? "").ToLower();

            if (expectedTitle != actualName)
            {
                return RedirectToActionPermanent("Detail", "Advertisement", new { id = model.id, adTitle = expectedTitle });
            }

            BreadCrumbConfiguration(SiteMaps.Current.CurrentNode, model);

            if (model.ListingImages.Count > 0)
            {
                var CoverImage = model.ListingImages.Where(l => l.IsCover).FirstOrDefault();

                if (CoverImage != null)
                {
                    model.CoverImage = CoverImage.Src.Replace("####size####", "s2");
                }
                else
                {
                    //Use first image as Cover image if not found
                    model.CoverImage = model.ListingImages.First().Src.Replace("####size####", "s2");
                    model.ListingImages.First().IsCover = true;
                }

                foreach (var item in model.ListingImages)
                {
                    if (!item.IsCover)
                    {
                        item.Thumnbnail = item.Src.Replace("####size####", "s0");
                        item.Src = item.Src.Replace("####size####", "s2");
                    }
                }
            }

            ViewData["EnquiryViewData"] = new App.Core.Models.SendEnquiryModel
            {
                Recipient = model.UserProfile,
                ListingId = model.id,
                ListingTitle = model.Title
            };

            return View(model);
        }

        protected void BreadCrumbConfiguration(ISiteMapNode node, ListingDetailViewModel model)
        {
            if (node != null && node.ParentNode != null)
            {
                node.Title = model.Title;

                foreach (var parent in node.Ancestors)
                {
                    if (parent.Key != "Home")
                    {
                        parent.RouteValues["LocationText"] = model.Location.Name.ToSeoUrl();
                    }

                    if (parent.Key == "Listing")
                    {
                        parent.Title = model.Location.Name;
                    }
                }
            }
        }
    }


}