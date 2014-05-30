using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Models;
using App.Core.Services;
using App.Ads.Code.Filters;
using App.Ads.Code.Membership;
using App.Core.Data;
using AutoMapper;
using Recaptcha.Web;
using Recaptcha.Web.Mvc;
using System.Threading.Tasks;

namespace App.Ads.Controllers
{
    public class EnquiryController : Controller
    {
        private readonly IListingService _listingService;
        private readonly IEnquiryService _enquiryService;

        public EnquiryController(IListingService listingService, IEnquiryService enquiryService)
        {
            _listingService = listingService;
            _enquiryService = enquiryService;
        }

        //
        // GET: /Enquiry/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(SendEnquiryModel model)
        {
            if (!ModelState.IsValid)
            {
                var errorList = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

                return Json(new { IsSuccess = false, Content = errorList }, JsonRequestBehavior.AllowGet);
            }

            RecaptchaVerificationHelper recaptcha = this.GetRecaptchaVerificationHelper();

            //Verify ReCaptcha
            if (String.IsNullOrEmpty(recaptcha.Response))
            {
                return Json(new { IsSuccess = false, Content = "Captcha answer cannot be empty." }, JsonRequestBehavior.AllowGet);
            }

            RecaptchaVerificationResult recaptchaResult = await recaptcha.VerifyRecaptchaResponseTaskAsync();

            if (recaptchaResult != RecaptchaVerificationResult.Success)
            {
                return Json(new { IsSuccess = false, Content = "Incorrect captcha answer." }, JsonRequestBehavior.AllowGet);
            }

            var listing = _listingService.GetListingById(model.ListingId);

            if (listing == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid Listing");
            }

            model.ListingTitle = listing.Title;
            model.Recipient = listing.UserProfile;

            _enquiryService.Save(Mapper.Map<SendEnquiryModel, Enquiry>(model));

            _enquiryService.SendEnquiry(model);

            return Json(new { IsSuccess = true, Content = "Your enquiry has been successfully sent to the seller." }, JsonRequestBehavior.AllowGet);
        }
    }
}