using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using App.Core.Services;
using App.Core.Models;
using App.Core.ViewModel;
using App.Core.Data;
using App.Ads.Code.Filters;
using App.Ads.Code.Helpers;
using App.Ads.ViewModel;
using MvcSiteMapProvider;
using AutoMapper;
using PagedList;

namespace App.Ads.Controllers
{
    public class SearchController : BaseController
    {
        private readonly ISearchService searchService;
        private readonly ICategoryService categoryService;
        private readonly IRegionService regionService;
        private readonly IReferenceService referenceService;

        private const int PAGE_SIZE = 10;

        public SearchController()
        {
            this.searchService = DependencyResolver.Current.GetService<ISearchService>();
            this.categoryService = DependencyResolver.Current.GetService<ICategoryService>();
            this.regionService = DependencyResolver.Current.GetService<IRegionService>();
            this.referenceService = DependencyResolver.Current.GetService<ReferenceService>();
        }

        // GET: Search
        //[SEOSearch]
        public ActionResult Index(SearchModel searchModel, string sort, int? page = 1)
        {
            var objSearch = TempData["tempSearch"];

            if (objSearch != null)
                searchModel = (SearchModel)objSearch;

            RebindForm();

            ViewBag.DateSortParm = String.IsNullOrEmpty(sort) ? "date" : "";
            ViewBag.PriceSortParm = sort == "price" ? "price_desc" : "price";

            if (!ValidateSearch(ref searchModel))
            {
                var error = new Tuple<PagedList.IPagedList<SearchViewModel>, SearchModel>(new List<SearchViewModel>().ToPagedList((int)page, PAGE_SIZE), searchModel);
                return View(error);
            }

            var searchResult = searchService.Search(searchModel, sort);

            BreadCrumbConfiguration(SiteMaps.Current.CurrentNode, searchModel);

            switch (sort)
            {
                case "date":
                    searchResult = searchResult.OrderBy(s => s.PostedDate);
                    ViewBag.DateSortIcon = "fa fa-chevron-down";
                    ViewBag.DateSortTooltips = "Show most recent first";
                    ViewBag.PriceSortIcon = "fa fa-chevron-up";
                    ViewBag.PriceSortTooltips = "Show cheapest first";
                    break;
                case "price":
                    searchResult = searchResult.OrderBy(s => s.Price);
                    ViewBag.PriceSortIcon = "fa fa-chevron-up";
                    ViewBag.PriceSortTooltips = "Show cheapest first";
                    ViewBag.DateSortIcon = "fa fa-chevron-down";
                    ViewBag.DateSortTooltips = "Show most recent first";
                    break;
                case "price_desc":
                    searchResult = searchResult.OrderByDescending(s => s.Price);
                    ViewBag.PriceSortIcon = "fa fa-chevron-down";
                    ViewBag.PriceSortTooltips = "Show price from highest to lowerest";
                    ViewBag.DateSortIcon = "fa fa-chevron-down";
                    ViewBag.DateSortTooltips = "Show most recent first";
                    break;
                default:  // date desc 
                    searchResult = searchResult.OrderByDescending(s => s.PostedDate);
                    ViewBag.DateSortIcon = "fa fa-chevron-up";
                    ViewBag.DateSortTooltips = "Sort from oldest to newest";
                    ViewBag.PriceSortIcon = "fa fa-chevron-down";
                    ViewBag.PriceSortTooltips = "Show cheapest first";
                    break;
            }

            ViewBag.CurrentSort = sort;

            var model = Mapper.Map<List<Listing>, List<SearchViewModel>>(searchResult.ToList());

            string areaText = searchModel.aid != null ? regionService.Find((int)searchModel.aid).Name : "";

            string listingType = searchModel.type != null ? referenceService.Find(searchModel.type).Name : "";

            ViewBag.SearchTitle = ListingHelper.GetSearchTitle(searchModel.Keyword, searchModel.CategoryText.DecodeSeoUrl(), areaText, searchModel.LocationText.DecodeSeoUrl(), listingType);

            var tuple = new Tuple<PagedList.IPagedList<SearchViewModel>, SearchModel>(model.ToPagedList((int)page, PAGE_SIZE), searchModel);

            return View(tuple);
        }

        [HttpPost]
        public ActionResult Search(SearchModel searchModel, int? page)
        {
            //searchModel.Keyword = string.IsNullOrEmpty(searchModel.Keyword2) ? searchModel.Keyword : searchModel.Keyword2;

            if (searchModel.lid > 0)
            {
                RegionZone regionZone = regionService.Find((int)searchModel.lid);
                searchModel.LocationText = regionZone.Name;
            }
            else
            {
                searchModel.LocationText = "malaysia";
            }

            if (searchModel.cid > 0)
            {
                RefCategory refCategory = categoryService.Find((int)searchModel.cid);
                searchModel.CategoryText = refCategory.Name;
            }

            TempData["tempSearch"] = searchModel;

            return RedirectToRoute("Search", new
            {
                controller = "Search",
                action = "Index",
                LocationText = searchModel.LocationText.ToSeoUrl(),
                CategoryText = searchModel.CategoryText.ToSeoUrl(),
                Keyword = searchModel.Keyword,
                type = searchModel.type,
                cid = searchModel.cid,
                lid = searchModel.lid,
                aid = searchModel.aid,
                page = page
            });
        }

        [HttpGet]
        public JsonResult GetArea(int? locationId)
        {
            Ads.Models.ResponseModel responseModel = new Ads.Models.ResponseModel();

            if (locationId != null)
            {
                List<RegionZone> regionZoneList = regionService.GetRegionByParentId((int)locationId)
                                                    .OrderBy(r => r.Name)
                                                    .ToList();

                responseModel = new Ads.Models.ResponseModel
                {
                    StatusCode = (int)System.Net.HttpStatusCode.OK,
                    Data = regionZoneList.Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.id.ToString()
                    })
                };

            }

            return Json(responseModel, JsonRequestBehavior.AllowGet);
        }

        protected void RebindForm()
        {
            ViewBag.ListingTypeList = referenceService.GetByType("LT").ToList();
            ViewBag.Categories = categoryService.GetCategories();
            ViewBag.LocationList = regionService.Get()
                                        .Where(r => r.ParentId == null)
                                        .OrderBy(r => r.Sort)
                                        .Select(r => new SelectListItem
                                        {
                                            Text = r.Name,
                                            Value = r.id.ToString()
                                        });
        }

        protected bool ValidateSearch(ref SearchModel model)
        {

            if (string.IsNullOrEmpty(model.LocationText))
            {
                ModelState.AddModelError("Error", ErrorConstant.INVALID_QUERY_STRING);
                return false;
            }

            if (model.LocationText.ToLower() != "malaysia")
            {
                RegionZone parentLocation = regionService.GetParentRegionZoneByName(model.LocationText.DecodeSeoUrl());

                if (parentLocation == null)
                {
                    ModelState.AddModelError("Error", ErrorConstant.INVALID_QUERY_STRING);
                    return false;
                }
                else
                {
                    model.lid = parentLocation.id;
                }
            }

            if (!string.IsNullOrEmpty(model.CategoryText))
            {
                RefCategory refCategory = categoryService.FindByName(model.CategoryText.DecodeSeoUrl());

                if (refCategory == null)
                {
                    ModelState.AddModelError("Error", ErrorConstant.INVALID_QUERY_STRING);
                    return false;
                }

                model.cid = refCategory.id;
                model.CategoryIds = categoryService.GetSubCategory(refCategory.id);
            }

            return ModelState.IsValid;
        }

        protected void BreadCrumbConfiguration(ISiteMapNode node, SearchModel searchModel)
        {
            if (node != null && node.ParentNode != null)
            {
                if (node.Key == "Listing")
                {
                    node.Title = searchModel.LocationText.DecodeSeoUrl();
                }
                else
                {
                    foreach (var parent in node.Ancestors)
                    {
                        if (parent.Key == "Listing")
                        {
                            parent.Title = searchModel.LocationText.DecodeSeoUrl();
                        }
                    }
                }
            }
        }

        
    }
}


//int? outersId = null;
//RefCategory selectedCategory = null;
//int categoryId = 0;

//var allCategories = categoryService.GetAll();

//var parentCategories = new List<RefCategory>();

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

//ViewData["categories"] = new TreeCategories
//{
//    Seed = outersId != null ? outersId : null,
//    Categories = allCategories,
//    SelectedId = categoryId == 0 ? (int?)null : categoryId,
//    ParentCategories = parentCategories.Count == 0 ? null : parentCategories.OrderBy(o => o.ParentID)
//};