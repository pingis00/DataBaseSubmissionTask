using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class PostalCodeValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt.");
        }

        string postalCode = value.ToString()!;

        if (!Regex.IsMatch(postalCode, @"^\d{3}(?:\s?\d{2})?$"))
        {
            return new ValidationResult(false, "ogiltigt postnummer: Exempel: 26478, 264 78");
        }

        return ValidationResult.ValidResult;
    }
}
