using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class NameValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {


        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt.");
        }

        string name = value.ToString()!;

        if (!Regex.IsMatch(name, @"^[a-zA-ZåäöÅÄÖ]{2,}$"))
        {
            return new ValidationResult(false, "Namnet måsta vara 2 tecken långt och får bara innehålla bokstäver.");
        }

        return ValidationResult.ValidResult;
    }
}
