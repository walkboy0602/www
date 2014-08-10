using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ExpressiveAnnotations.ConditionalAttributes;
using App.Core.Utility;

namespace App.Core.ViewModel
{
    public class PublishListingViewModel
    {
        [BooleanRequired(ErrorMessage = "Please agree to the Terms & Conditions and Privacy Policy.")]
        public bool AgreesWithTerms { get; set; }

        [Required(ErrorMessage="Please select your ad duration.")]
        public string DurationCode { get; set; }

        public string FeatureCode { get; set; }
    }
}
