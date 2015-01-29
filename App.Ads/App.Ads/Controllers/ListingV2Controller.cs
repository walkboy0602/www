using App.Ads.Code.BO;
using App.Ads.Code.Security;
using App.Ads.Models;
using App.Ads.ViewModel;
using App.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Ads.Controllers
{
    public class ListingV2Controller : BaseController
    {
        private readonly IListingBO listingBO;
        private readonly ICommonBO commonBO;

        public ListingV2Controller()
        {
            this.listingBO = DependencyResolver.Current.GetService<IListingBO>();
            this.commonBO = DependencyResolver.Current.GetService<ICommonBO>();
        }

        //
        // GET: /Listing2/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult New()
        {
            return View();
        }

        // Get : /Listing2/Save/2
        public ActionResult Save(int id)
        {
            var listing = listingBO.Find(id);

            if (listing == null)
            {
                return RedirectToAction("Manage");
            }

            ListingGeneralInfo generalInfo = new ListingGeneralInfo()
            {
                Title = listing.Title
            };

            ListingCreateViewModel model = new ListingCreateViewModel()
            {
                GeneralInfo = generalInfo
            };


            ViewBag.LocationList = commonBO.GetLocationSelectList();
            ViewBag.AreaList = commonBO.GetAreaSelectList();
            
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Save(ListingCreateViewModel model)
        {
            ViewBag.PropertyTypeList = commonBO.GetLocationSelectList();
            return RedirectToAction("Listing", "Manage");
        }

        [ChildActionOnly]
        public ActionResult Property(ListingCreateViewModel model)
        {
            return PartialView("_property");
        }

        [HttpPost]
        public JsonResult Create(SelectCategoryVO request)
        {
            int ListingId = listingBO.Create(request, CurrentUser.CustomIdentity.UserId);

            SelectCategoryResponseVO selectCategory = new SelectCategoryResponseVO
            {
                RedirectUrl = Url.Action("Save", "ListingV2", new { id = "123" })
            };

            ResponseModel response = new ResponseModel
            {
                Body = selectCategory
            };

            return Json(response);
        }


    }
}