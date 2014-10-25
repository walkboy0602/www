using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Ads.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Url { get; set; }
        public int? ParentId { get; set; }
        public Boolean Disable { get; set; }
        public Boolean HasAccess { get; set; }
        public int ListingCount { get; set; }
        public Menu ParentMenu { get; set; }
    }
}