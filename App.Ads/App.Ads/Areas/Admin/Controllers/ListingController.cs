using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
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
                                .Where(s => s.Status == (int)XtEnum.ListingStatus.Processing)
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

        public JsonResult Approve(ActionModel model)
        {
            var listing = _listingService.GetListingById(model.ListingId);

            if (listing == null)
            {
                responseModel = new Ads.Models.ResponseModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Listing not exists."
                };
                return Json(responseModel);
            }

            if (listing.Status == (int)XtEnum.ListingStatus.Online)
            {
                responseModel = new Ads.Models.ResponseModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Listing already published."
                };
                return Json(responseModel);
            }

            listing.PostedDate = listing.PostedDate == null ? DateTime.Now : listing.PostedDate;
            listing.PostingEndDate = listing.PostingEndDate == null ? DateTime.Now.AddDays((int)listing.Duration) : listing.PostingEndDate;
            listing.Status = (int)XtEnum.ListingStatus.Online;
            listing.LastAction = "Posted";
            listing.LastActionBy = CurrentUser.CustomIdentity.UserId;
            _listingService.Save(listing);

            responseModel = new Ads.Models.ResponseModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Listing successfully approved."
            };

            return Json(responseModel);

            //return new JsonResult { Data = new {fieldName = "Word", error = "Not really a word!" } };
        }

        [HttpPost]
        public JsonResult Reject(ActionModel model)
        {
            var listing = _listingService.GetListingById(model.ListingId);

            if (listing == null)
            {
                responseModel = new Ads.Models.ResponseModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Listing not exists."
                };
                return Json(responseModel);
            }


            if (listing.Status == (int)XtEnum.ListingStatus.Rejected)
            {
                responseModel = new Ads.Models.ResponseModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Listing already rejected."
                };
                return Json(responseModel);
            }

            listing.LastAction = "Reject";
            listing.LastActionBy = CurrentUser.CustomIdentity.UserId;
            listing.Status = (int)XtEnum.ListingStatus.Rejected;
            listing.RejectCode = model.RejectModel.RejectCode;

            _listingService.Save(listing);

            responseModel = new Ads.Models.ResponseModel
            {
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Listing successfully rejected."
            };
            return Json(responseModel);

        }
    }
}