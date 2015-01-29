using App.Ads.ViewModel;
using App.Core.Data;
using App.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Ads.Code.BO
{
    public interface IListingBO
    {
        Listing Find(int ListingId);
        Listing Find(int ListingId, int UserId);
        int Create(SelectCategoryVO model, int UserId);
    }

    public class ListingBO : IListingBO
    {
        private readonly IListingService _listingService;
        private readonly ICommonService _commonService;
        private readonly ICategoryService _categoryService;

        public ListingBO(IListingService listingService, ICommonService commonService, ICategoryService categoryService)
        {
            this._listingService = listingService;
            this._commonService = commonService;
            this._categoryService = categoryService;
        }

        Listing IListingBO.Find(int ListingId)
        {
            return _listingService.GetListingById(ListingId);
        }

        Listing IListingBO.Find(int ListingId, int UserId)
        {
            return _listingService.GetListingById(ListingId, UserId);
        }

        int IListingBO.Create(SelectCategoryVO model, int UserId)
        {
            var category = _categoryService.Find(model.CategoryId);

            string templateType = category.TemplateType.FirstOrDefault().ToString();

            string templateName = _commonService.GetTemplateTypeList()
                .Where(s => s.Code == templateType && s.ListType == model.ListType).Select(t => t.TemplateName).FirstOrDefault();


            Listing listing = new Listing
            {
                CategoryId = model.CategoryId,
                ListType = model.ListType,
                CreateBy = UserId,
                CreateDate = DateTime.Now,
                TemplateName = templateName
            };

            return 12;
            //return _listingService.Create(listing);
        }
    }
}