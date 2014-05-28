using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using App.Core.Models;

namespace App.Ads.Controllers.api
{
    public class EnquiryController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage Post([FromBody]SendEnquiryModel model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Your enquiry has been successfully sent to the seller.");
        }
    }
}
