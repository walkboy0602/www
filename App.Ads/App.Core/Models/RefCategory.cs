using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App.Core.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace App.Core.Data
{
    public partial class RefCategory
    {
        public IEnumerable<RefCategory> AllSubcategories()
        {
            yield return this;
            foreach (var directSubcategory in SubCategories)
                foreach (var subcategory in directSubcategory.AllSubcategories())
                {
                    yield return subcategory;
                }
        }
    }

}
