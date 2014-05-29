using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using App.Core.Models;
using App.Core.Services;
using App.Ads.Code.Filters;
using App.Ads.Code.Membership;
using App.Core.Data;
using AutoMapper;


namespace App.Ads.Controllers.api
{
    public class EnquiryController : ApiController
    {
        private readonly IListingService _listingService;
        private readonly IEnquiryService _enquiryService;

        public EnquiryController(IListingService listingService, IEnquiryService enquiryService)
        {
            _listingService = listingService;
            _enquiryService = enquiryService;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody]SendEnquiryModel model)
        {
            var listing = _listingService.GetListingById(model.ListingId);

            if (listing == null)
            {
                Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bad Request.");
            }

            model.ListingTitle = listing.Title;
            model.Recipient = listing.UserProfile;

            _enquiryService.Save(Mapper.Map<SendEnquiryModel, Enquiry>(model));

            _enquiryService.SendEnquiry(model);

            return Request.CreateResponse(HttpStatusCode.OK, "Your enquiry has been successfully sent to the seller.");
        }
    }
}
