using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using App.Core.Services;
using System.Web.Mvc;

namespace App.Ads.Code.Helpers
{
    public class CommonHelper
    {
        private readonly IReferenceService referenceService;

        public CommonHelper()
        {
            this.referenceService = DependencyResolver.Current.GetService<IReferenceService>();
        }

        public string GetReferenceLabel(string code)
        {
            var refTable = referenceService.Find(code);

            return refTable == null ? "" : refTable.Name;
        }
    }
}