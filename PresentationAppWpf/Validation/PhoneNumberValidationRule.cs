using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class PhoneNumberValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null)
        {
            return new ValidationResult(false, "Telefonnummer får inte vara tomt.");
        }

        string phoneNumber = value.ToString()!;

        if (!Regex.IsMatch(phoneNumber, @"^(\+\d{1,3})?(\d{3,4}\s?){2,3}\d{3,4}$"))
        {
            return new ValidationResult(false, "Telefonnumret är ogiltigt. Exempel 0702789698, +46702789698, 0702 789 698, +46 70 278 9698");
        }

        return ValidationResult.ValidResult;
    }
}
