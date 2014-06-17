﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;

namespace App.Core.ViewModel
{
    public class CategoryGroup
    {
        public RefCategory Value { get; set; }
        public IEnumerable<CategoryGroup> Children { get; set; }
    }

    public class RegionZoneGroup
    {
        public RegionZone Value { get; set; }
        public IEnumerable<RegionZoneGroup> Children { get; set; }
    }

    public class TreeCategories
    {
        public int? Seed { get; set; }
        public int? SelectedId { get; set; }
        public IEnumerable<RefCategory> ParentCategories { get; set; }
        public IEnumerable<RefCategory> Categories { get; set; }
    }
}
