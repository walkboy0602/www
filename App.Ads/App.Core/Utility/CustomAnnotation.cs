using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace App.Core.Utility
{
    public class BooleanRequiredAttribute : ValidationAttribute, IClientValidatable
    {

        public bool Value
        {
            get;
            set;
        }

        public override bool IsValid(object value)
        {
            return value != null && value is bool && (bool)value == true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(
               ModelMetadata metadata,
               ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName()),
                ValidationType = "booleanrequired"
            };
        }
    }

    public struct RegularExpressions
    {
        public static string EmailAddress = @"^[a-zA-Z0-9_\+-]+(\.[a-zA-Z0-9_\+-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.([a-zA-Z]{2,4})$";
    }

    public class ValidEmailAddressAttribute : RegularExpressionAttribute
    {
        public ValidEmailAddressAttribute() : base(RegularExpressions.EmailAddress) { }
    }


}
