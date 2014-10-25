using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace App.Ads.Models
{
    public class Menu
    {
        public Menu()
        {
            MenuItems = new List<MenuItem>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<MenuItem> MenuItems { get; set; }
    }
}