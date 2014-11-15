using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace App.Ads.Models
{
    public class BaseModel
    {

        public int UserId { get; set; }

        private ModelStateDictionary _modelState = new ModelStateDictionary();
        public ModelStateDictionary ModelState
        {
            get
            {
                return _modelState;
            }
            set
            {
                _modelState = value;
            }
        }
    }
}