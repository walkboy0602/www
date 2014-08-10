using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using App.Core.Data;
using App.Core.ViewModel;
using AutoMapper;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using App.Ads.Code.Helpers;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Web.Mvc.Filters;

namespace App.Ads.Controllers
{
    public class AdvertisementController : Controller
    {
        private AdsDBEntities db = new AdsDBEntities();

        private const string S3Domain = "http://assets.monsteritem.com/";

        // GET: Ad
        //[MvcSiteMapNode(Title = "Advertisement Details", PreservedRouteParameters = "id")]
        //[SiteMapTitle("CategoryId", Target = AttributeTarget.ParentNode)]
        [HttpGet]
        public ActionResult Index(int id, string adTitle, string a)
        {
            var listing = db.Listings.Find(id);

            var model = Mapper.Map<Listing, AdDetailViewModel>(listing);

            if (!string.IsNullOrEmpty(a))
            {
                model.Action = a;
            }

            string expectedTitle = model.Title.ToSeoUrl();
            string actualName = (adTitle ?? "").ToLower();

            if (expectedTitle != actualName)
            {
                return RedirectToActionPermanent("Index", "Advertisement", new { id = model.id, adTitle = expectedTitle });
            }

            if (model.LocationParentId != 0)
            {
                model.ParentLocation = db.RegionZones.FirstOrDefault(x => x.id == model.LocationParentId).Name;
            }

            var node = SiteMaps.Current.CurrentNode;
            if (node != null && node.ParentNode != null)
            {
                node.Title = listing.Title;
            }

            int dayDiff = (DateTime.Now - model.CreateDate).Days;

            model.CreateDateText = dayDiff == 0 ? "Today" : dayDiff + " day ago";

            if (model.ListingImages.Count > 0)
            {
                var CoverImage = model.ListingImages.Where(l => l.IsCover).FirstOrDefault();

                if (CoverImage != null)
                {
                    model.CoverImage = S3Domain + CoverImage.Src.Replace("####size####", "s2");
                }
                else
                {
                    //Use first image as Cover image if not found
                    model.CoverImage = S3Domain + model.ListingImages.First().Src.Replace("####size####", "s2");
                    model.ListingImages.First().IsCover = true;
                }

                foreach (var item in model.ListingImages)
                {
                    if (!item.IsCover)
                    {
                        item.Thumnbnail = S3Domain + item.Src.Replace("####size####", "s0");
                        item.Src = S3Domain + item.Src.Replace("####size####", "s2");
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


    }


}