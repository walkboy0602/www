using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using App.Core.Services;
using App.Core.ViewModel;
using App.Ads.Code.Security;
using App.Ads.Code.Membership;
using App.Ads.Controllers;
using App.Ads.Areas.Admin.Models;
using App.Core.Data;

namespace App.Ads.Areas.Admin.Controllers
{
    [Authorize]
    public class ListingController : BaseController
    {
        private readonly IListingService _listingService;
        private readonly IReferenceService _referenceService;

        public ListingController()
        {
            this._listingService = DependencyResolver.Current.GetService<IListingService>();
            this._referenceService = DependencyResolver.Current.GetService<IReferenceService>();
        }

        // GET: Admin/Listing
        [UserRoleAuthorize(Roles = "SuperAdmin, Admin")]
        public ActionResult Index()
        {
            var listings = _listingService.GetAll()
                                .Where(s => s.Status == (int)XtEnum.ListingStatus.Pending)
                                .OrderByDescending(o => o.LastUpdate)
                                .ToList();


            return View(listings);
        }

        [ChildActionOnly]
        public ActionResult Action(int id)
        {
            ActionModel model = new ActionModel();

            model.ListingId = id;
            if (CurrentUser.IsInRole("SuperAdmin, Admin"))
            {
                ViewBag.ReasonList = _referenceService.GetByType("REASON")
                                        .Select(r => new SelectListItem()
                                            {
                                                Text = r.Name,
                                                Value = r.Code
                                            });
                return PartialView(model);
            }

            return Content("");
        }

        [HttpPost]
        public JsonResult Reject(ActionModel model)
        {
            var listing = _listingService.GetListingById(model.ListingId);

            if (listing != null)
            {
                listing.LastAction = "Reject";
                listing.LastActionBy = CurrentUser.CustomIdentity.UserId;
                listing.Status = (int)XtEnum.ListingStatus.Rejected;

                _listingService.Save(listing);
            }
            return Json("Success");
        }
    }
}