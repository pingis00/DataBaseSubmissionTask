using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class PasswordValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Fältet får inte vara tomt.");
        }

        string password = value.ToString()!;

        if (!Regex.IsMatch(password, @"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*])(?=\S+$).{8,}$"))
        {
            return new ValidationResult(false, "Lösenordet måste innehålla minst 1 stor bokstav, 1 liten bokstav, 1 siffra, 1 specialtecken och vara minst 8 tecken långt.");
        }

        return ValidationResult.ValidResult;
    }
}
