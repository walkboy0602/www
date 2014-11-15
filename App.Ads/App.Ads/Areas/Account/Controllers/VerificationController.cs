using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Services;
using App.Ads.Controllers;
using App.Core.Data;
using WebMatrix.WebData;
using System.Net;
using App.Ads.Models;

namespace App.Ads.Areas.Account.Controllers
{
    [Authorize]
    public class VerificationController : BaseController
    {
        private readonly IUserService userService;
        private readonly BO.IAccountBO accountBO;

        public VerificationController(IUserService userService)
        {
            this.userService = userService;
            this.accountBO = DependencyResolver.Current.GetService<BO.IAccountBO>();
        }

        //
        // GET: /Account/Verify/
        public ActionResult Index()
        {
            Membership membership = userService.GetMembership(User.Identity.Name);
            return View(membership);
        }

        [HttpPost, ActionName("SendEmail")]
        public JsonResult SendVerificationEmail()
        {
            BaseModel model = new BaseModel();

            model.UserId = CurrentUser.CustomIdentity.UserId;

            if (!accountBO.ResendEmail(model))
            {
                responseModel = new Ads.Models.ResponseModel
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = model.ModelState.Keys.SelectMany(k => model.ModelState[k].Errors).Select(m => m.ErrorMessage).FirstOrDefault()
                };
                return Json(responseModel);
            }

            return Json(new { StatusCode = (int)HttpStatusCode.OK, Message = MessageConstant.EMAIL_RESEND });
        }

        [AllowAnonymous]
        public ActionResult Success(string type = null)
        {
            ViewBag.Message = "Your email is successfully verified.";
            return View();
        }


        [AllowAnonymous]
        public ActionResult Confirmation(Guid? guid)
        {
            if (!guid.HasValue)
            {
                ViewBag.Message = "Activation code is invalid.";
                //return RedirectToAction("Index", "Error", new { area = "" });
            }

            WebSecurity.ConfirmAccount(guid.Value.ToString());
           
            return RedirectToAction("Success", "Verification", new { area = "Account" });

            //var membership = this.userService.GetMembershipByConfirmToken(guid.Value.ToString(), withUserProfile: true);
            //WebSecurity.Login(membership.UserProfile.UserName, membership.EmailConfirmationToken);

            //return RedirectToAction("Index", "Verification", new { area = "Account" });
        }
    }
}