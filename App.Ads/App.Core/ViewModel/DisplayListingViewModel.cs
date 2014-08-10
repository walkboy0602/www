using System;
using App.Core.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace App.Core.ViewModel
{

    public class DisplayListingViewModel
    {
     
        public int id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateDateText { get; set; }

        public int Status { get; set; }

        public string StatusTooltip { get; set; }

        private string ReplaceHTML(string str)
        {
            return Regex.Replace(str, @"<[^>]*>", string.Empty);
        }

    }


}
