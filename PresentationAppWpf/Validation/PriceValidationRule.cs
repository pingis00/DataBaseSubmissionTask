using System.Globalization;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

internal class PriceValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || !decimal.TryParse(value.ToString(), NumberStyles.Any, cultureInfo, out decimal price))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt.");
        }
        if (price > 0)
        {
            return ValidationResult.ValidResult;
        }
        else
        {
            return new ValidationResult(false, "Pris måste vara större än 0.");
        }
    }
}
