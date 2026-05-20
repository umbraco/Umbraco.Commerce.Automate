using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Adds a product to an existing Commerce order.
/// </summary>
[Action("umbracoCommerce.addProductToOrder", "Add Product to Order",
    Description = "Adds a product to an existing Commerce order.",
    Group = "Commerce",
    Icon = "icon-shopping-basket-alt-2",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class AddProductToOrderAction : ActionBase<AddProductToOrderSettings, AddProductToOrderOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public AddProductToOrderAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IUnitOfWorkProvider uowProvider,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _orderService = orderService;
        _uowProvider = uowProvider;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<AddProductToOrderSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (string.IsNullOrWhiteSpace(settings.ProductReference))
        {
            return ActionResult.Failed(
                new ArgumentException("A Product Reference is required."),
                StepRunErrorCategory.Validation);
        }

        if (settings.Quantity <= 0)
        {
            return ActionResult.Failed(
                new ArgumentException("Quantity must be greater than zero."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(order.StoreId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        await _uowProvider.ExecuteAsync(async uow =>
        {
            var writableOrder = await order.AsWritableAsync(uow);

            if (string.IsNullOrWhiteSpace(settings.ProductVariantReference))
            {
                await writableOrder.AddProductAsync(settings.ProductReference, settings.Quantity);
            }
            else
            {
                await writableOrder.AddProductAsync(settings.ProductReference, settings.ProductVariantReference, settings.Quantity);
            }

            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new AddProductToOrderOutput
        {
            OrderId = orderId,
            ProductReference = settings.ProductReference,
            ProductVariantReference = settings.ProductVariantReference,
            Quantity = settings.Quantity,
        });
    }
}
