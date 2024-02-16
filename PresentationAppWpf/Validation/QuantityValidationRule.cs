using System.Globalization;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class QuantityValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || !int.TryParse(value.ToString(), out int quantity))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt.");
        }
        if (quantity >= 0)
        {
            return ValidationResult.ValidResult;
        }
        else
        {
            return new ValidationResult(false, "Antal måste vara ett icke-negativt heltal.");
        }
    }
}

