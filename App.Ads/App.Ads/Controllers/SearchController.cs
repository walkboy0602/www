using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Services;
using App.Core.Models;
using App.Core.ViewModel;
using App.Core.Data;

namespace App.Ads.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;
        private readonly ICategoryService categoryService;

        public SearchController()
        {
            this.searchService = DependencyResolver.Current.GetService<ISearchService>();
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
        }

        // GET: Search
        public ActionResult Index(SearchModel searchModel)
        {
            int? outersId = null;
            RefCategory selectedCategory = null;
            int categoryId = 0;

            var allCategories = categoryService.GetAll();

            var parentCategories = new List<RefCategory>();

            if (searchModel.Category != null)
            {
                selectedCategory = allCategories.Where(d => d.Description.ToLower() == searchModel.Category.ToLower()).FirstOrDefault();

                if (selectedCategory != null)
                {
                    categoryId = selectedCategory.id;

                    outersId = allCategories.Where(d => d.id == selectedCategory.id).FirstOrDefault().ParentID;
                    if (outersId != null)
                    {
                        parentCategories.Add(allCategories.Where(d => d.id == outersId).FirstOrDefault());
                    }

                    for (int i = 0; i < parentCategories.Count; i++)
                    {
                        if (parentCategories[i].ParentID != null)
                        {
                            outersId = allCategories.Where(d => d.id == selectedCategory.id).FirstOrDefault().ParentID;
                            parentCategories.Add(allCategories.Where(d => d.id == parentCategories[i].ParentID).FirstOrDefault());
                        }
                    }
                }
                else
                {
                    //Not Found
                }
            }

            ViewData["categories"] = new TreeCategories
            {
                Seed = outersId != null ? outersId : null,
                Categories = allCategories,
                SelectedId = categoryId == 0 ? (int?)null : categoryId,
                ParentCategories = parentCategories.Count == 0 ? null : parentCategories.OrderBy(o => o.ParentID)
            };

            IEnumerable<Listing> result = null;

            if (categoryId != 0)
            {
                result = searchService.SearchByCategory(categoryId);
            }
            else
            {
                result = searchService.SearchAds(searchModel);
            }

            var tuple = new Tuple<IEnumerable<Listing>, SearchModel>(result, searchModel);

            return View(tuple);
        }
    }
}