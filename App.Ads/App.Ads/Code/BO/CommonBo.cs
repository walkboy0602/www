using App.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Ads.Code.BO
{

    public interface ICommonBO
    {
        IEnumerable<SelectListItem> GetLocationSelectList();
        IEnumerable<SelectListItem> GetAreaSelectList(int locationId = 0);

        IEnumerable<SelectListItem> GetSelectListByRefType(string Type, string selectedValue = null);
    }

    public class CommonBo : ICommonBO
    {
        private readonly IRegionService _regionService;
        private readonly IReferenceService _referenceService;

        public CommonBo(IRegionService regionService, IReferenceService referenceService)
        {
            this._regionService = regionService;
            this._referenceService = referenceService;
        }

        IEnumerable<SelectListItem> ICommonBO.GetLocationSelectList()
        {
            return _regionService.Get()
                    .Where(r => r.ParentId == null)
                    .OrderBy(r => r.Sort)
                    .Select(r => new SelectListItem
                    {
                        Text = r.Name,
                        Value = r.id.ToString()
                    });
        }

        IEnumerable<SelectListItem> ICommonBO.GetAreaSelectList(int locationId)
        {
            return _regionService.GetRegionByParentId(locationId)
                                    .OrderBy(r => r.Sort)
                                    .Select(r => new SelectListItem
                                    {
                                        Text = r.Name,
                                        Value = r.id.ToString()
                                    });
        }

        IEnumerable<SelectListItem> ICommonBO.GetSelectListByRefType(string Type, string selectedValue)
        {
            return _referenceService.GetByType(Type)
                                         .Select(r => new SelectListItem()
                                         {
                                             Text = r.Name,
                                             Value = r.Code,
                                             Selected = r.Code == selectedValue
                                         });
        }
    }
}