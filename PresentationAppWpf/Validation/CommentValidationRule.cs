﻿using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PresentationAppWpf.Validation;

public class CommentValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return new ValidationResult(false, "Kommentaren får inte vara tom.");
        }

        string comment = value.ToString()!;

        if (!Regex.IsMatch(comment, @"^[\p{L}\p{N}\p{P}\p{S} ]{10,}$"))
        {
            return new ValidationResult(false, "Kommentaren måste vara minst 10 tecken lång och får innehålla bokstäver, siffror och specialtecken.");
        }

        return ValidationResult.ValidResult;
    }
}
