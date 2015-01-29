using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Ads.ViewModel
{
    public class CategoryViewModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public Nullable<int> ParentID { get; set; }
        public Nullable<int> Sort { get; set; }
        public string FaIcon { get; set; }
        public string DisplayName { get; set; }
        public string ListType { get; set; }
    }
}