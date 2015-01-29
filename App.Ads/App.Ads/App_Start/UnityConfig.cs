using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;
using System.Net.Mail;
using App.Core.Services;
using App.Ads.Code.BO;

namespace App.Ads
{
    //This Config is use for Web Api
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            
            // register all your components with the container here
            // it is NOT necessary to register your controllers
            
            // e.g. container.RegisterType<ITestService, TestService>();


            // BO layer

            container.RegisterType<IListingBO, ListingBO>();


            // Service layer

            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IConfigService, ConfigService>();
            container.RegisterType<IImageService, ImageService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<IReferenceService, ReferenceService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IRegionService, RegionService>();
            container.RegisterType<IListingService, ListingService>();
            container.RegisterType<IAWSService, AWSService>();
            container.RegisterType<IAzureService, AzureService>();
            container.RegisterType<IEnquiryService, EnquiryService>();
            container.RegisterType<SmtpClient>(new InjectionConstructor());


            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

        }
    }
}