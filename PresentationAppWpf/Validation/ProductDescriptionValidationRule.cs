using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation
{
    public class ProductDescriptionValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string productDescription = value.ToString()!;

            if (!Regex.IsMatch(productDescription, @"^.{0,500}$"))
            {
                return new ValidationResult(false, "Produktbeskrivningen får vara max 500 tecken lång");
            }

            return ValidationResult.ValidResult;
        }
    }
}
