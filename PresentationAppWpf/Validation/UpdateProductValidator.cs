using ApplicationCore.ProductCatalog.Dtos;
using System.Globalization;

namespace PresentationAppWpf.Validation;

public class UpdateProductValidator(UpdateProductDto updateProductDto, Action<string> showMessage)
{
    protected readonly UpdateProductDto _updateProductDto = updateProductDto;
    protected readonly Action<string> _showMessage = showMessage;

    public bool ValidateUpdate()
    {
        return ValidateArticleNumber() &&
               ValidateTitle() &&
               ValidateProductDescription() &&
               ValidateBrandName() &&
               ValidateCategoryName();
    }

    private bool ValidateTitle()
    {
        var titleValidationRule = new ProductNameValidationRule();
        var titleValidationResult = titleValidationRule.Validate(_updateProductDto.Title, CultureInfo.CurrentCulture);
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
        var brandValidationResult = brandValidationRule.Validate(_updateProductDto.Brand.BrandName, CultureInfo.CurrentCulture);
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
        var categoryValidationResult = categoryValidationRule.Validate(_updateProductDto.Category.CategoryName, CultureInfo.CurrentCulture);
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
        var articleNumberValidationResult = articleNumberValidationRule.Validate(_updateProductDto.ArticleNumber, CultureInfo.CurrentCulture);
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
        var productDescriptionValidationResult = productDescriptionValidationRule.Validate(_updateProductDto.ProductDescription, CultureInfo.CurrentCulture);
        if (!productDescriptionValidationResult.IsValid)
        {
            _showMessage(productDescriptionValidationResult.ErrorContent?.ToString() ?? "Ett okänt fel uppstod vid validering av produktinformationen.");
            return false;
        }
        return true;
    }
}
