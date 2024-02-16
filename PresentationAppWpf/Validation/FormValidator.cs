using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.ProductCatalog.Dtos;
using PresentationAppWpf.Mvvm.ViewModels;
using System.Globalization;

namespace PresentationAppWpf.Validation;

public class FormValidator(ICustomerDto customerDto, Action<string> showMessage)
{
    private readonly ICustomerDto _customerDto = customerDto;
    private readonly Action<string> _showMessage = showMessage;

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
        return !string.IsNullOrWhiteSpace(_customerDto.FirstName) &&
               !string.IsNullOrWhiteSpace(_customerDto.LastName) &&
               !string.IsNullOrWhiteSpace(_customerDto.Email) &&
               !string.IsNullOrWhiteSpace(_customerDto.PhoneNumber) &&
               !string.IsNullOrWhiteSpace(_customerDto.Address.StreetName) &&
               !string.IsNullOrWhiteSpace(_customerDto.Address.PostalCode) &&
               !string.IsNullOrWhiteSpace(_customerDto.Address.City) &&
               _customerDto.Role != null &&
               !string.IsNullOrWhiteSpace(_customerDto.Role.RoleName) &&
               _customerDto.ContactPreference != null &&
               !string.IsNullOrWhiteSpace(_customerDto.ContactPreference.PreferredContactMethod);
    }

    private bool ValidateEmail()
    {
        var emailValidationRule = new EmailValidationRule();
        var emailValidationResult = emailValidationRule.Validate(_customerDto.Email, CultureInfo.CurrentCulture);
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
        var phoneNumberlValidationResult = phoneNumberlValidationRule.Validate(_customerDto.PhoneNumber, CultureInfo.CurrentCulture);
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
        var firstNameValidationResult = firstNameValidationRule.Validate(_customerDto.FirstName, CultureInfo.CurrentCulture);
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
        var lastNameValidationResult = lastNameValidationRule.Validate(_customerDto.LastName, CultureInfo.CurrentCulture);
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
        var cityValidationResult = cityValidationRule.Validate(_customerDto.Address.City, CultureInfo.CurrentCulture);
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
        var postalCodeValidationResult = postalCodeValidationRule.Validate(_customerDto.Address.PostalCode, CultureInfo.CurrentCulture);
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
        var streetNameValidationResult = streetNameValidationRule.Validate(_customerDto.Address.StreetName, CultureInfo.CurrentCulture);
        if (!streetNameValidationResult.IsValid)
        {
            _showMessage(streetNameValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av adressen.");
            return false;
        }
        return true;
    }
}



