using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace App.Core.Models
{
    public class SearchModel
    {
        public string ConditionCode { get; set; }

        [Display(Name = "CategoryId")]
        public int? cid { get; set; }
        [Display(Name = "LocationId")]
        public int? lid { get; set; }
        [Display(Name = "AreaId")]
        public int? aid { get; set; }

        [Display(Name = "ListingType")]
        public string type { get; set; }

        public List<int> CategoryIds { get; set; }
        public string Keyword { get; set; }

        /// <summary>
        /// Purpose of having Keyword2 because it get overwritten by keyword search box
        /// </summary>
        public string Keyword2 { get; set; }

        public string CategoryText { get; set; }
        public string LocationText { get; set; }

    }
}
