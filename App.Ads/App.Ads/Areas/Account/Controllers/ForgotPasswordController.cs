using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Ads.Areas.Account.Models;
using App.Ads.Areas.Account.BO;
using App.Core.Services;
using App.Ads.Controllers;

namespace App.Ads.Areas.Account.Controllers
{
    public class ForgotPasswordController : BaseController
    {
        private readonly IAccountBO accountBO;

        public ForgotPasswordController()
        {
            this.accountBO = DependencyResolver.Current.GetService<IAccountBO>();
        }

        // GET: Account/ForgotPassword
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                if (accountBO.ForgotPassword(model))
                {
                    ViewBag.SuccessMsg = MessageConstant.PASSWORD_REQUEST;
                }
                else
                {
                    ViewBag.ModelState = model.ModelState;
                }

            }

            return View(model);
        }
    }
}