using ApplicationCore.ProductCatalog.Dtos;
using System.Globalization;

namespace PresentationAppWpf.Validation;

public class CreateProductValidator(CompleteProductDto completeProductDto, Action<string> showMessage)
{
    private readonly CompleteProductDto _completeProductDto = completeProductDto;
    private readonly Action<string> _showMessage = showMessage;

    public bool ValidateProductCreation()
    {
        if (!Validatefields())
        {
            _showMessage("Vänligen fyll i alla obligatoriska fält..");
            return false;
        }

        if (!ValidateTitle() || !ValidateBrandName() || !ValidateCategoryName() ||
            !ValidateArticleNumber() || !ValidateProductDescription() || !ValidateQuantity()
            || !ValidatePrice())
        {
            return false;
        }
        return true;
    }

    private bool Validatefields()
    {
        return !string.IsNullOrWhiteSpace(_completeProductDto.Title) &&
               !string.IsNullOrWhiteSpace(_completeProductDto.ProductDescription) &&
               !string.IsNullOrWhiteSpace(_completeProductDto.Brandname) &&
               !string.IsNullOrWhiteSpace(_completeProductDto.CategoryName);

    }

    private bool ValidateTitle()
    {
        var titleValidationRule = new NameValidationRule();
        var titleValidationResult = titleValidationRule.Validate(_completeProductDto.Title, CultureInfo.CurrentCulture);
        if (!titleValidationResult.IsValid)
        {
            _showMessage(titleValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av titel.");
            return false;
        }
        return true;
    }

    private bool ValidateBrandName()
    {
        var brandValidationRule = new NameValidationRule();
        var brandValidationResult = brandValidationRule.Validate(_completeProductDto.Brandname, CultureInfo.CurrentCulture);
        if (!brandValidationResult.IsValid)
        {
            _showMessage(brandValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av varumärke.");
            return false;
        }
        return true;
    }

    private bool ValidateCategoryName()
    {
        var categoryValidationRule = new NameValidationRule();
        var categoryValidationResult = categoryValidationRule.Validate(_completeProductDto.CategoryName, CultureInfo.CurrentCulture);
        if (!categoryValidationResult.IsValid)
        {
            _showMessage(categoryValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av varumärke.");
            return false;
        }
        return true;
    }

    private bool ValidateArticleNumber()
    {
        var articleNumberValidationRule = new ArticleNumberValidationRule();
        var articleNumberValidationResult = articleNumberValidationRule.Validate(_completeProductDto.CategoryName, CultureInfo.CurrentCulture);
        if (!articleNumberValidationResult.IsValid)
        {
            _showMessage(articleNumberValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av artikelnummer.");
            return false;
        }
        return true;
    }

    private bool ValidateProductDescription()
    {
        var productDescriptionValidationRule = new ProductDescriptionValidationRule();
        var productDescriptionValidationResult = productDescriptionValidationRule.Validate(_completeProductDto.CategoryName, CultureInfo.CurrentCulture);
        if (!productDescriptionValidationResult.IsValid)
        {
            _showMessage(productDescriptionValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av produktinformationen.");
            return false;
        }
        return true;
    }
    private bool ValidateQuantity()
    {
        var quantityValidationRule = new QuantityValidationRule();
        var quantityValidationResult = quantityValidationRule.Validate(_completeProductDto.CategoryName, CultureInfo.CurrentCulture);
        if (!quantityValidationResult.IsValid)
        {
            _showMessage(quantityValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av antalet.");
            return false;
        }
        return true;
    }
    private bool ValidatePrice()
    {
        var priceValidationRule = new PriceValidationRule();
        var priceValidationResult = priceValidationRule.Validate(_completeProductDto.CategoryName, CultureInfo.CurrentCulture);
        if (!priceValidationResult.IsValid)
        {
            _showMessage(priceValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av produktinformationen.");
            return false;
        }
        return true;
    }



}
