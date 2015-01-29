using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using App.Core.Models;
using App.Core.Data;
using System.Web.Caching;
using System.Web;

namespace App.Core.Services
{
    public interface ICommonService
    {
        IEnumerable<TemplateType> GetTemplateTypeList();

        List<SelectListItem> GetSalutationSelectList();
    }

    public class CommonService : ICommonService
    {
        private AdsDBEntities db;

        public CommonService()
        {
            db = new AdsDBEntities();
        }

        IEnumerable<TemplateType> ICommonService.GetTemplateTypeList()
        {
            string cacheKey = "templateType";

            IEnumerable<TemplateType> templateType = null;

            if (HttpRuntime.Cache[cacheKey] != null)
            {
                templateType = (IEnumerable<TemplateType>)HttpRuntime.Cache[cacheKey];
            }
            else
            {
                templateType = db.TemplateTypes.ToList();
            }

            HttpRuntime.Cache.Insert(cacheKey, templateType, null, DateTime.Now.AddMinutes(240), Cache.NoSlidingExpiration);

            return templateType;
        }

        List<SelectListItem> ICommonService.GetSalutationSelectList()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Add(new SelectListItem() { Selected = false, Text = "Mr", Value = "Mr" });
            list.Add(new SelectListItem() { Selected = false, Text = "Mrs", Value = "Mrs" });
            list.Add(new SelectListItem() { Selected = false, Text = "Miss", Value = "Miss" });
            list.Add(new SelectListItem() { Selected = false, Text = "Ms", Value = "Ms" });
            list.Add(new SelectListItem() { Selected = false, Text = "Dr", Value = "Dr" });

            return list;
        }
    }
}
