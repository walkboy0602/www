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

namespace App.Ads.Controllers
{
    public class AdvertisementController : Controller
    {
        private AdsDBEntities db = new AdsDBEntities();

        private const string S3Domain = "http://assets.monsteritem.com/";

        // GET: Ad
        public ActionResult Index(int aid)
        {
            var listing = db.Listings.Find(aid);

            var model = Mapper.Map<Listing, AdDetailViewModel>(listing);

            if (model.LocationParentId != 0)
            {
                model.ParentLocation = db.RegionZones.FirstOrDefault(x => x.id == model.LocationParentId).Name;
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


            return View(model);
        }
    }
}