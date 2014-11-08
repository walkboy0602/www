using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.ComponentModel.DataAnnotations;

namespace App.Core.ViewModel
{
    public class ImageModel
    {
        public Uri UrlDomain = new Uri(string.Format("https://{0}.blob.core.windows.net/", 
            ConfigurationManager.AppSettings["StorageAccountName"].ToString()) + "images/");
    }

    public class ListingImageViewModel : ImageModel
    {
        public int id { get; set; }
        public int ListingId { get; set; }
        public string FileName { get; set; }
        public bool IsCover { get; set; }
        public Nullable<byte> Sort { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string Src { get; set; }
        public string Description { get; set; }
    }

    public class EditImageViewModel
    {
        [Required]
        public int id { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsCover { get; set; }
    }
}
