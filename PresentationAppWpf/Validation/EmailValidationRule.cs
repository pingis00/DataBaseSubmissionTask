using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation
{
    public class EmailValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
            {
                return new ValidationResult(false, "Email får inte vara tomt.");
            }

            string email = value.ToString()!;

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$"))
            {
                return new ValidationResult(false, "Email är inte i ett giltigt format. Exempel: anvandarnamn@example.com");
            }

            return ValidationResult.ValidResult;
        }
    }
}
