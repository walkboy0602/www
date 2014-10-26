using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using App.Ads.Code.Security;

namespace App.Ads.Controllers
{
    public class ExceptionController : BaseController
    {
        public ActionResult Error()
        {
            Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ViewBag.ErrorID = ExceptionUtility.ErrorID;
            return View();
        }

        public ActionResult PageNotFound()
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            return View();
        }

        // Shhh .. secret test method .. ooOOooOooOOOooohhhhhhhh
        public ActionResult ThrowError()
        {
            throw new NotImplementedException("Pew ^ Pew");
        }
    }
}