using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using App.Ads.Code.Membership;
using App.Ads.Code.Security;

namespace App.Ads
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            UnityConfig.RegisterComponents();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrapper.Initialise();
            AutoMapperConfig.Configure();
            AuthConfig.RegisterAuth();
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Get the exception object.
            Exception exception = Server.GetLastError();
            
            string ErrorID = DateTime.Now.ToString("yyyyMMddhhmmss") + "E" + new Random().Next(100000, 999999);
            ExceptionUtility.ErrorID = ErrorID;
            ExceptionUtility.LogException(exception, Request, "HttpErrorPage", ErrorID);

            //InvokeExceptionController(sender, e, exception);
        }

        private void InvokeExceptionController(object sender, EventArgs e, Exception exception)
        {

            var httpContext = ((MvcApplication)sender).Context;

            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            var currentController = " ";
            var currentAction = " ";

            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }

            var controller = new App.Ads.Controllers.ExceptionController();
            var routeData = new RouteData();
            var action = "Error";

            if (exception is HttpException)
            {
                var httpEx = exception as HttpException;

                switch (httpEx.GetHttpCode())
                {
                    default:
                        action = "Error";
                        break;
                }
            }

            httpContext.ClearError();
            httpContext.Response.Clear();
            httpContext.Response.StatusCode = exception is HttpException ? ((HttpException)exception).GetHttpCode() : 500;
            httpContext.Response.TrySkipIisCustomErrors = true;
            routeData.Values["controller"] = "Exception";
            routeData.Values["action"] = action;

            // Call target Controller and pass the routeData.

            if (httpContext.Response.StatusCode == 500)
            {
                IController exceptionController = new App.Ads.Controllers.ExceptionController();
                exceptionController.Execute(new RequestContext(
                     new HttpContextWrapper(Context), routeData));

                controller.ViewData.Model = new HandleErrorInfo(exception, currentController, currentAction);
                ((IController)controller).Execute(new RequestContext(new HttpContextWrapper(httpContext), routeData));
            }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                var identity = new CustomIdentity(HttpContext.Current.User.Identity);
                var principal = new CustomPrincipal(identity);
                HttpContext.Current.User = principal;
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            //if (Response.StatusCode == 401)
            //{
            //    Response.ClearContent();
            //    Response.RedirectToRoute("ErrorHandler", (RouteTable.Routes["ErrorHandler"] as Route).Defaults);
            //}
        }
    }
}