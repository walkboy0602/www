using App.Ads.Code.BO;
using App.Ads.Code.Membership;
using App.Ads.Code.Security;
using App.Ads.ViewModel;
using App.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App.Ads.Controllers.api
{
    [CustomAuthorize]
    public class ListingController : ApiController
    {
        //private readonly IListingBO _listingBO;

        //public ListingController(IListingBO listingBO)
        //{
        //    _listingBO = listingBO;
        //}

        [HttpPost]
        public HttpResponseMessage Post([FromBody]SelectCategoryVO request)
        {
            CustomIdentity identity = User.ToCustomPrincipal().CustomIdentity;

            //_listingBO.Create(request, identity.UserId);

            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        //[HttpPost]
        //public HttpResponseMessage Post(SelectCategoryVO request)
        //{
        //    CustomIdentity identity = User.ToCustomPrincipal().CustomIdentity;

        //    //_listingBO.Create(request, identity.UserId);

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

    }
}