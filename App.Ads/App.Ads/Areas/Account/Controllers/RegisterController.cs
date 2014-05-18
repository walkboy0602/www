using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using App.Ads.Areas.Account.Models;

namespace App.Ads.Areas.Account.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Account/Register
        public ActionResult Index()
        {

            ViewBag.Salutation = GetSalutationSelectList();

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(RegisterModel model)
        {
            //TODO: Override OnActionExecuting
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {

                    var token = WebSecurity.CreateUserAndAccount(model.Email, model.Password,
                        propertyValues: new
                        {
                            Salutation = model.Salutation,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            Mobile = model.Mobile
                        }, requireConfirmationToken: true);

                    //this.userService.SendAccountActivationMail(model.Email);

                    return RedirectToAction("success", "register", new { email = model.Email, area = "account" });
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }
            ViewBag.Salutation = GetSalutationSelectList();

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/Register/Success

        public ActionResult Success(string email)
        {
            ViewData["email"] = email;
            return View();
        }


        #region Helpers

        private List<SelectListItem> GetSalutationSelectList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Selected = false, Text = "Mr", Value = "Mr" });
            list.Add(new SelectListItem() { Selected = false, Text = "Mrs", Value = "Mrs" });
            list.Add(new SelectListItem() { Selected = false, Text = "Miss", Value = "Miss" });
            list.Add(new SelectListItem() { Selected = false, Text = "Ms", Value = "Ms" });
            list.Add(new SelectListItem() { Selected = false, Text = "Dr", Value = "Dr" });

            return list;
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion
    }
}