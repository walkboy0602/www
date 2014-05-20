using System;
using App.Core.Data;
using System.ComponentModel.DataAnnotations;

namespace App.Core.ViewModel
{

    public class DisplayListingViewModel
    {
     
        public int id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreateDate { get; set; }

        public int Status { get; set; }

    }
}
