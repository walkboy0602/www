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
                foreach (var item in model.ListingImages)
                {
                    if (!item.IsCover)
                    {
                        model.CoverImage = "http://assets.monsteritem.com/" + item.Src.Replace("####size####", "s2");
                    }

                    item.Src = "http://assets.monsteritem.com/" + item.Src.Replace("####size####", "s2");
                }
            }
            else
            {
                model.CoverImage = "";
            }

            return View(model);
        }
    }
}