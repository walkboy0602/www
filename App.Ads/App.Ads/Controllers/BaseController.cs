using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Ads.Code.Membership;

namespace App.Ads.Controllers
{
    public class BaseController : Controller
    {
        public CustomPrincipal CurrentUser;

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = User.ToCustomPrincipal();
            }
            else
            {
                CurrentUser = null;
            }
        }

    }
}