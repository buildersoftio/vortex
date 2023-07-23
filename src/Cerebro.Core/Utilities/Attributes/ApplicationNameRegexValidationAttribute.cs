using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Cerebro.Core.Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ApplicationNameRegexValidationAttribute : ValidationAttribute
    {
        private readonly string _pattern;

        public ApplicationNameRegexValidationAttribute()
        {
            _pattern = @"[^a-zA-Z0-9_.\-]";
        }
        public ApplicationNameRegexValidationAttribute(string pattern)
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
