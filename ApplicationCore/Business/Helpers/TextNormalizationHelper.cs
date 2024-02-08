namespace ApplicationCore.Business.Helpers;

public static class TextNormalizationHelper
{
    public static OperationResult<string> NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return OperationResult<string>.Failure("Ingen text angiven.");

        var words = text.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        var normalizedText = string.Join(" ", words);
        return OperationResult<string>.Success("Texten normaliserades framgångsrikt.", normalizedText);
    }

    public static OperationResult<string> FormatSwedishPhoneNumber(string phoneNumber)
    {
        string digitsOnly = new(phoneNumber.Where(char.IsDigit).ToArray());

        if (digitsOnly.StartsWith("46") && digitsOnly.Length == 11)
        {
            digitsOnly = "0" + digitsOnly[2..];
        }

        if (digitsOnly.Length != 10)
        {
            return OperationResult<string>.Failure("Ogiltigt telefonnummer. Ett svenskt telefonnummer ska bestå av 10 siffror.");
        }
        string formattedNumber = $"{digitsOnly[..3]} {digitsOnly[3..6]} {digitsOnly[6..8]} {digitsOnly[8..]}";
        return OperationResult<string>.Success("Telefonnummer formaterades framgångsrikt.", formattedNumber);
    }

    public static OperationResult<string> FormatSwedishPostalCode(string postalCode)
    {
        string digitsOnly = new string(postalCode.Where(char.IsDigit).ToArray());

        if (digitsOnly.Length != 5)
        {
            return OperationResult<string>.Failure("Ogiltig postkod. En svensk postkod ska bestå av 5 siffror.");
        }

        string formattedPostalCode = $"{digitsOnly[..3]} {digitsOnly[3..]}";

        return OperationResult<string>.Success("Postkoden formaterades framgångsrikt.", formattedPostalCode);
    }
}
