using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Updates the stock level of a product.
/// </summary>
[Action("umbracoCommerce.updateStock", "Update Stock",
    Description = "Sets, increases, or decreases the stock level of a product.",
    Group = "Commerce",
    Icon = "icon-box",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class UpdateStockAction : ActionBase<UpdateStockSettings, UpdateStockOutput>
{
    private readonly IStockService _stockService;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public UpdateStockAction(
        ActionInfrastructure infrastructure,
        IStockService stockService,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _stockService = stockService;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<UpdateStockSettings>();

        if (!Guid.TryParse(settings.StoreId, out var storeId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Store ID is required."),
                StepRunErrorCategory.Validation);
        }

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(storeId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        if (string.IsNullOrWhiteSpace(settings.ProductReference))
        {
            return ActionResult.Failed(
                new ArgumentException("A Product Reference is required."),
                StepRunErrorCategory.Validation);
        }

        var operation = settings.Operation?.Trim();
        if (!string.Equals(operation, "Set", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(operation, "Increase", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(operation, "Decrease", StringComparison.OrdinalIgnoreCase))
        {
            return ActionResult.Failed(
                new ArgumentException("Operation must be one of: Set, Increase, Decrease."),
                StepRunErrorCategory.Validation);
        }

        var hasVariant = !string.IsNullOrWhiteSpace(settings.ProductVariantReference);
        var variantRef = settings.ProductVariantReference ?? string.Empty;

        if (string.Equals(operation, "Set", StringComparison.OrdinalIgnoreCase))
        {
            if (hasVariant)
            {
                await _stockService.SetStockAsync(storeId, settings.ProductReference, variantRef, settings.Value, cancellationToken);
            }
            else
            {
                await _stockService.SetStockAsync(storeId, settings.ProductReference, settings.Value, cancellationToken);
            }
        }
        else if (string.Equals(operation, "Increase", StringComparison.OrdinalIgnoreCase))
        {
            if (hasVariant)
            {
                await _stockService.IncreaseStockAsync(storeId, settings.ProductReference, variantRef, settings.Value, cancellationToken);
            }
            else
            {
                await _stockService.IncreaseStockAsync(storeId, settings.ProductReference, settings.Value, cancellationToken);
            }
        }
        else
        {
            if (hasVariant)
            {
                await _stockService.ReduceStockAsync(storeId, settings.ProductReference, variantRef, settings.Value, cancellationToken);
            }
            else
            {
                await _stockService.ReduceStockAsync(storeId, settings.ProductReference, settings.Value, cancellationToken);
            }
        }

        var newStock = hasVariant
            ? await _stockService.GetStockAsync(storeId, settings.ProductReference, variantRef, cancellationToken)
            : await _stockService.GetStockAsync(storeId, settings.ProductReference, cancellationToken);

        return Success(new UpdateStockOutput
        {
            StoreId = storeId,
            ProductReference = settings.ProductReference,
            ProductVariantReference = settings.ProductVariantReference,
            Operation = operation,
            Value = settings.Value,
            NewStockLevel = newStock,
        });
    }
}
