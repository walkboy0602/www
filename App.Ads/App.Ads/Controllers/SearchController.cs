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
using App.Ads.Models;
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

        private const int PAGE_SIZE = 15;

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

            ViewBag.DateSortParm = String.IsNullOrEmpty(sort) ? "date" : "";
            ViewBag.PriceSortParm = sort == "price" ? "price_desc" : "price";

            if (!ValidateSearch(ref searchModel))
            {
                var error = new Tuple<PagedList.IPagedList<SearchViewModel>, SearchModel>(new List<SearchViewModel>().ToPagedList((int)page, PAGE_SIZE), searchModel);
                return View(error);
            }

            var searchResult = searchService.Search(searchModel, sort);

            BreadCrumbConfiguration(SiteMaps.Current.CurrentNode, searchModel);

            Menu menu = new Menu();

            GenerateMenu(menu, searchModel.cid);

            List<int> fds = menu.MenuItems.Select(s => s.Id).ToList();

            RebindForm(fds, searchModel.cid);

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

        protected void RebindForm(List<int> categoryIds, int? cid)
        {
            if (categoryIds != null && categoryIds.Count() > 0 && cid != null)
            {
                //ViewBag.Categories = new SelectList(categoryService.GetCategoriesOptGroupByIds(categoryIds), "Id", "Name", "Category", 1);

                ViewBag.Categories = categoryService.GetCategoriesByIds(categoryIds);
            }
            else
            {
                ViewBag.Categories = new SelectList(categoryService.GetCategoriesOptGroupByParentId(null), "Id", "Name", "Category", 1);
            }

            ViewBag.ListingTypeList = referenceService.GetByType("LT").ToList();
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

        protected void GenerateMenu(Menu menu, int? categoryId)
        {
            if (categoryId != null)
            {
                var category = categoryService.Find((int)categoryId);

                menu.Id = category.id;

                if (category != null)
                {
                    PopulateMenu(menu, category.id, true);
                }
            }
            else
            {
                PopulateMenuItem(menu, null);
            }

            ViewBag.Menu = menu;
        }

        protected void PopulateMenu(Menu menu, int categoryId, bool showItems)
        {
            var category = categoryService.Find(categoryId);

            MenuItem menuItem = new MenuItem();

            menuItem.Id = category.id;
            menuItem.Name = category.Name;
            menuItem.ParentId = category.ParentID;

            if (showItems)
            {
                PopulateMenuItem(menu, (int)category.id);
            }

            menu.MenuItems.Add(menuItem);

            if (category.ParentID != null)
            {
                PopulateMenu(menu, (int)category.ParentID, false);
            }
        }

        protected void PopulateMenuItem(Menu menu, int? categoryId)
        {
            var categories = categoryService.GetByParentID(categoryId);

            foreach (var cat in categories)
            {
                MenuItem menuItem = new MenuItem();

                menuItem.Id = cat.id;
                menuItem.Name = cat.Name;
                menuItem.ParentId = cat.ParentID;

                menu.MenuItems.Add(menuItem);
            }
        }

    }
}
