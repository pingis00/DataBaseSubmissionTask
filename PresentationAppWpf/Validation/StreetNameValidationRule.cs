using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class StreetNameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt.");
        }

        string password = value.ToString()!;

        if (!Regex.IsMatch(password, @"^[a-zA-ZåäöÅÄÖ0-9 ]{2,}$"))
        {
            return new ValidationResult(false, "Adressen får endast bestå av bokstäver och siffror och måste vara minst 2 tecken långt");
        }

        return ValidationResult.ValidResult;
    }
}
