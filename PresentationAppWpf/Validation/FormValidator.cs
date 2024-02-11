using ApplicationCore.Business.Dtos;
using PresentationAppWpf.Mvvm.ViewModels;
using System.Globalization;

namespace PresentationAppWpf.Validation;

public class FormValidator
{
    private readonly CustomerRegistrationDto _customerRegistrationDto;
    private readonly Action<string> _showMessage;

    public FormValidator(CustomerRegistrationDto customerRegistrationDto, Action<string> showMessage)
    {
        _customerRegistrationDto = customerRegistrationDto;
        _showMessage = showMessage;
    }

    public bool ValidateForm()
    {
        if (!fieldsAreNotEmpty())
        {
            _showMessage("Vänligen fyll i alla obligatoriska fält..");
            return false;
        }

        if (!ValidateEmail() || !ValidatePhoneNumber() || !ValidateFirstName() ||
            !ValidateLastName() || !ValidateCity() || !ValidatePostalCode() ||
            !ValidateStreetName())
        {

            return false;
        }
        return true;
    }

    private bool fieldsAreNotEmpty()
    {
        return !string.IsNullOrWhiteSpace(_customerRegistrationDto.FirstName) &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.LastName) &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.Email) &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.PhoneNumber) &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.Address.StreetName) &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.Address.PostalCode) &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.Address.City) &&
               _customerRegistrationDto.Role != null &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.Role.RoleName) &&
               _customerRegistrationDto.ContactPreference != null &&
               !string.IsNullOrWhiteSpace(_customerRegistrationDto.ContactPreference.PreferredContactMethod);
    }

    private bool ValidateEmail()
    {
        var emailValidationRule = new EmailValidationRule();
        var emailValidationResult = emailValidationRule.Validate(_customerRegistrationDto.Email, CultureInfo.CurrentCulture);
        if (!emailValidationResult.IsValid)
        {
            _showMessage(emailValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av e-postadressen.");
            return false;
        }
        return true;
    }

    private bool ValidatePhoneNumber()
    {
        var phoneNumberlValidationRule = new PhoneNumberValidationRule();
        var phoneNumberlValidationResult = phoneNumberlValidationRule.Validate(_customerRegistrationDto.PhoneNumber, CultureInfo.CurrentCulture);
        if (!phoneNumberlValidationResult.IsValid)
        {
            _showMessage(phoneNumberlValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av telefonnumret.");
            return false;
        }
        return true;
    }
    private bool ValidateFirstName()
    {
        var firstNameValidationRule = new NameValidationRule();
        var firstNameValidationResult = firstNameValidationRule.Validate(_customerRegistrationDto.FirstName, CultureInfo.CurrentCulture);
        if (!firstNameValidationResult.IsValid)
        {
            _showMessage(firstNameValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av förnamnet.");
            return false;
        }
        return true;
    }
    private bool ValidateLastName()
    {
        var lastNameValidationRule = new NameValidationRule();
        var lastNameValidationResult = lastNameValidationRule.Validate(_customerRegistrationDto.LastName, CultureInfo.CurrentCulture);
        if (!lastNameValidationResult.IsValid)
        {
            _showMessage(lastNameValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av efternamnet.");
            return false;
        }
        return true;
    }
    private bool ValidateCity()
    {
        var cityValidationRule = new NameValidationRule();
        var cityValidationResult = cityValidationRule.Validate(_customerRegistrationDto.Address.City, CultureInfo.CurrentCulture);
        if (!cityValidationResult.IsValid)
        {
            _showMessage(cityValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av staden.");
            return false;
        }
        return true;
    }
    private bool ValidatePostalCode()
    {
        var postalCodeValidationRule = new PostalCodeValidationRule();
        var postalCodeValidationResult = postalCodeValidationRule.Validate(_customerRegistrationDto.Address.PostalCode, CultureInfo.CurrentCulture);
        if (!postalCodeValidationResult.IsValid)
        {
            _showMessage(postalCodeValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av postakoden.");
            return false;
        }
        return true;

    }
    private bool ValidateStreetName()
    {
        var streetNameValidationRule = new StreetNameValidationRule();
        var streetNameValidationResult = streetNameValidationRule.Validate(_customerRegistrationDto.Address.StreetName, CultureInfo.CurrentCulture);
        if (!streetNameValidationResult.IsValid)
        {
            _showMessage(streetNameValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av adressen.");
            return false;
        }
        return true;
    }
}



