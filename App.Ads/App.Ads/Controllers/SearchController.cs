using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Services;
using App.Core.Models;
using App.Core.ViewModel;
using App.Core.Data;
using App.Ads.ViewModel;
using MvcSiteMapProvider;
using AutoMapper;
using PagedList;

namespace App.Ads.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService searchService;
        private readonly ICategoryService categoryService;
        private readonly IRegionService regionService;

        public SearchController()
        {
            this.searchService = DependencyResolver.Current.GetService<ISearchService>();
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.regionService = DependencyResolver.Current.GetService<IRegionService>();
        }

        // GET: Search
        public ActionResult Index(SearchModel searchModel, int? page)
        {
            int? outersId = null;
            RefCategory selectedCategory = null;
            int categoryId = 0;

            var allCategories = categoryService.GetAll();

            var parentCategories = new List<RefCategory>();

            //if (searchModel.Category != null)
            //{
            //    selectedCategory = allCategories.Where(d => d.Description.ToLower() == searchModel.Category.ToLower()).FirstOrDefault();

            //    if (selectedCategory != null)
            //    {
            //        categoryId = selectedCategory.id;

            //        outersId = allCategories.Where(d => d.id == selectedCategory.id).FirstOrDefault().ParentID;
            //        if (outersId != null)
            //        {
            //            parentCategories.Add(allCategories.Where(d => d.id == outersId).FirstOrDefault());
            //        }

            //        for (int i = 0; i < parentCategories.Count; i++)
            //        {
            //            if (parentCategories[i].ParentID != null)
            //            {
            //                outersId = allCategories.Where(d => d.id == selectedCategory.id).FirstOrDefault().ParentID;
            //                parentCategories.Add(allCategories.Where(d => d.id == parentCategories[i].ParentID).FirstOrDefault());
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //Not Found
            //    }
            //}

            ViewData["categories"] = new TreeCategories
            {
                Seed = outersId != null ? outersId : null,
                Categories = allCategories,
                SelectedId = categoryId == 0 ? (int?)null : categoryId,
                ParentCategories = parentCategories.Count == 0 ? null : parentCategories.OrderBy(o => o.ParentID)
            };

            List<Listing> searchResult = null;

            if (categoryId != 0)
            {
                searchResult = searchService.SearchByCategory(categoryId).ToList();
            }
            else
            {
                searchResult = searchService.Get(searchModel).ToList();
            }

            var model = new List<SearchViewModel>();

            model = Mapper.Map<List<Listing>, List<SearchViewModel>>(searchResult);

            foreach (var item in model)
            {
                if (item.ListingImages.Count > 0)
                {
                    item.CoverImage = item.ListingImages
                                        .Where(i => i.IsCover)
                                        .Select(i => i.Src).FirstOrDefault();

                    if (string.IsNullOrEmpty(item.CoverImage)) item.CoverImage = item.ListingImages.Select(i => i.Src).FirstOrDefault();
                }

                if (item.RegionZone != null && item.RegionZone.ParentId != null)
                {
                    item.ParentLocation = regionService.GetRegionZoneById(item.RegionZone.ParentId).Name;
                }
            }

            int pageSize = 20;
            int pageNumber = (page ?? 1);

            var tuple = new Tuple<PagedList.IPagedList<SearchViewModel>, SearchModel>(model.ToPagedList(pageNumber, pageSize), searchModel);

            return View(tuple);
        }

    }
}