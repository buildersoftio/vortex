using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Vortex.Core.Utilities.Attributes
{
    public class AddressRegexValidationAttribute : ValidationAttribute
    {
        private readonly string _pattern;
        public AddressRegexValidationAttribute()
        {
            _pattern = @"^\/[a-zA-Z0-9_/-]+$";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string stringValue = value.ToString();

                if (!Regex.IsMatch(stringValue, _pattern))
                {
                    return new ValidationResult(ErrorMessage);
                }

                if (stringValue.EndsWith("/"))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
