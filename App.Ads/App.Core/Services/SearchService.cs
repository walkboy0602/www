using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;
using App.Core.Models;

namespace App.Core.Services
{
    public interface ISearchService
    {
        IEnumerable<Listing> SearchAds(SearchModel searchModel);

        IEnumerable<Listing> SearchByCategory(int category);
    }

    public class SearchService : ISearchService
    {
        private AdsDBEntities db;
        private readonly ICategoryService categoryService;

        public SearchService(ICategoryService categoryService)
        {
            this.db = new AdsDBEntities();
            this.categoryService = categoryService;
        }

        IEnumerable<Listing> ISearchService.SearchAds(SearchModel searchModel)
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
            var subIds = categoryService.GetAll()
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
