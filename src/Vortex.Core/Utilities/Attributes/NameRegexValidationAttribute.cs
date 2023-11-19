using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Vortex.Core.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NameRegexValidationAttribute : ValidationAttribute
    {
        private readonly string _pattern;
        private readonly string _addressPattern;

        public NameRegexValidationAttribute()
        {
            _pattern = @"[^a-zA-Z0-9_.\-]";
            _addressPattern = @"^\/[a-zA-Z0-9_/]+$";
        }
        public NameRegexValidationAttribute(string pattern)
        {
            _pattern = pattern;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string stringValue = value.ToString();

                if (Regex.IsMatch(stringValue, _pattern))
                {
                    return new ValidationResult(ErrorMessage);
                }

                if (Regex.IsMatch(stringValue, @"^\.*$"))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
