using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Services;

namespace App.Ads.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IListingService _listingService;

        public HomeController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this._listingService = DependencyResolver.Current.GetService<IListingService>();
        }

        public ActionResult Index()
        {
            ViewBag.LatestAdList = _listingService.GetLatestAds(10);
            ViewBag.ParentCategoryList = categoryService.Get().Where(c => c.ParentID == null).OrderBy(c => c.Sort);
            ViewBag.CategoryList = categoryService.Get().OrderBy(c => c.Sort);
            return View();
        }
    }
}
