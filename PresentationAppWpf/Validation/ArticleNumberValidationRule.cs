using System.Globalization;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class ArticleNumberValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {

        if (value == null || !int.TryParse(value.ToString(), out int articleNumber))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt");
        }

        if (articleNumber >= 10000000 && articleNumber <= 99999999)
        {
            return ValidationResult.ValidResult;
        }
        else
        {
            return new ValidationResult(false, "Artikelnumret måste vara exakt 8 siffror långt.");
        }
    }
}
