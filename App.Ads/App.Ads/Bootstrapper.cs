using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Unity.Mvc4;
using System.Net.Mail;
using App.Core.Services;
using App.Ads.Areas.Account.Service;
using System.Linq;
using System.Web.Http;

namespace App.Ads
{
    public static class Bootstrapper
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();    

            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<ICommonService, CommonService>();
            container.RegisterType<IConfigService, ConfigService>();
            container.RegisterType<IEmailService, EmailService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IRegionService, RegionService>();
            container.RegisterType<IListingService, ListingService>();
            container.RegisterType<IAWSService, AWSService>();
            container.RegisterType<IEnquiryService, EnquiryService>();
            container.RegisterType<ISearchService, SearchService>();
            container.RegisterType<IAccountService, AccountService>();
            container.RegisterType<IFeatureService, FeatureService>();
            container.RegisterType<IReferenceService, ReferenceService>();
            container.RegisterType<ICacheService, CacheService>();
            container.RegisterType<SmtpClient>(new InjectionConstructor());


            RegisterTypes(container);

            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {

        }
    }
}