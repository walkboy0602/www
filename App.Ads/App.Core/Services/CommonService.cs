using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace App.Core.Services
{
    public interface ICommonService
    {
        List<SelectListItem> GetSalutationSelectList();
    }

    public class CommonService : ICommonService
    {
        public CommonService()
        {

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
