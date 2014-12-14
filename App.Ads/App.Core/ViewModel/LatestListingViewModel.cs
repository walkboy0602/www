using App.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.ViewModel
{
    public class LatestListingViewModel : ImageModel
    {
        public int id { get; set; }

        public string Title { get; set; }

        public DateTime? PostedDate { get; set; }

        public Nullable<decimal> Price { get; set; }

        public string CategoryName { get; set; }

        public string Location { get; set; }

        public string Area { get; set; }

        public string ImageSrc { get; set; }
    }
}
