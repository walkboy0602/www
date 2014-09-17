using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Services;

namespace App.Ads.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService categoryService;

        public HomeController()
        {
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
        }

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

            ViewBag.CategoryList = categoryService.Get().OrderBy(c => c.Sort);

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
