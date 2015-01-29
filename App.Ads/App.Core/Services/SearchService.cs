using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;
using App.Core.Models;
using System.Web.Mvc;
//using Binbin.Linq;
using LinqKit;
using System.Linq;

namespace App.Core.Services
{
    public interface ISearchService
    {
        IQueryable<Listing> Search(SearchModel model, string sort);

        IEnumerable<Listing> Get(SearchModel searchModel);

        IEnumerable<Listing> SearchByCategory(int category);
    }

    public class SearchService : ISearchService
    {
        private AdsDBEntities db = new AdsDBEntities();

        private readonly IRegionService regionService;
        private readonly ICategoryService categoryService;

        public SearchService(ICategoryService categoryService, IRegionService regionService)
        {
            this.categoryService = categoryService;
            this.regionService = regionService;
            db.Configuration.LazyLoadingEnabled = false;
        }

        IQueryable<Listing> ISearchService.Search(SearchModel model, string sort)
        {
            var predicate = PredicateBuilder.True<Listing>();

            // Type
            if (!string.IsNullOrEmpty(model.type))
            {
                predicate = predicate.And(p => p.ListType == model.type);
            }

            // Location
            if (model.lid > 0)
            {
                predicate = predicate.And(p => p.LocationId == model.lid);
            }

            // Area
            if (model.aid > 0)
            {
                predicate = predicate.And(p => p.AreaId == model.aid);
            }

            // Category
            if (model.CategoryIds != null && model.CategoryIds.Count > 0)
            {
                predicate = predicate.And(p => model.CategoryIds.Contains((int)p.CategoryId));
            }

            // Condition
            if (!string.IsNullOrEmpty(model.ConditionCode))
            {
                predicate = predicate.And(p => p.ConditionCode == model.ConditionCode);
            }

            // Keyword
            var predKeyword = PredicateBuilder.False<Listing>();
            if (!string.IsNullOrEmpty(model.Keyword))
            {
                string[] keywords = model.Keyword.Split(' ');

                foreach (var key in keywords)
                {
                    predKeyword = predKeyword.Or(p => p.Title.Contains(key));
                    predKeyword = predKeyword.Or(p => p.Keywords.Contains(key));
                    predKeyword = predKeyword.Or(p => p.RefCategory.Name.Contains(key) && p.RefCategory.isActive == true);
                }

                predicate = predicate.And(predKeyword.Expand());
            }

            predicate = predicate.And(p => p.Status == (int)App.Core.ViewModel.XtEnum.ListingStatus.Live);
    
            predicate = predicate.And(p => p.PostingEndDate >= DateTime.Now);

            var results = db.Listings
                            .Include("Location")
                            .Include("Area")
                            .Include("RefCategory")
                            .Include("ListingImages")
                            .AsExpandable()
                            .Where(predicate);

            return results;
        }

        IEnumerable<Listing> ISearchService.Get(SearchModel searchModel)
        {
            var listing = from l in db.Listings
                          select l;

            if (!string.IsNullOrEmpty(searchModel.Keyword))
            {
                listing = listing.Where(k => k.Title.Contains(searchModel.Keyword)
                                        || k.Keywords.Contains(searchModel.Keyword)
                                        || (k.RefCategory.Name.Contains(searchModel.Keyword) && k.RefCategory.isActive == true)
                                        );
            }

            return listing;
        }

        IEnumerable<Listing> ISearchService.SearchByCategory(int categoryId)
        {
            //Retrieve all sub category of the selected categoryId
            var subIds = categoryService.Get()
                                .First(c => c.id == categoryId)
                                .AllSubcategories()
                                .Select(x => x.id)
                                .ToList();

            var listing = db.Listings
                        .Where(l => subIds.Contains(l.RefCategory.id));

            return listing;
        }


    }
}
