using System.Text.Json.Nodes;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Fetches a single Commerce order by ID or by store + order number.
/// </summary>
[Action("umbracoCommerce.getOrder", "Get Order",
    Description = "Fetches a Commerce order by ID or by store + order number.",
    Group = "Commerce",
    Icon = "icon-shopping-basket",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class GetOrderAction : ActionBase<GetOrderSettings, GetOrderOutput>
{
    private readonly IOrderService _orderService;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public GetOrderAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _orderService = orderService;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<GetOrderSettings>();

        OrderReadOnly? order = null;

        if (!string.IsNullOrWhiteSpace(settings.OrderId))
        {
            if (!Guid.TryParse(settings.OrderId, out var orderId))
            {
                return ActionResult.Failed(
                    new ArgumentException("A valid Order ID is required."),
                    StepRunErrorCategory.Validation);
            }

            order = await _orderService.GetOrderAsync(orderId);
        }
        else if (!string.IsNullOrWhiteSpace(settings.OrderNumber))
        {
            if (!Guid.TryParse(settings.StoreId, out var storeId))
            {
                return ActionResult.Failed(
                    new ArgumentException("A valid Store ID is required when looking up by Order Number."),
                    StepRunErrorCategory.Validation);
            }

            order = await _orderService.GetOrderAsync(storeId, settings.OrderNumber);
        }
        else
        {
            return ActionResult.Failed(
                new ArgumentException("Either an Order ID or a Store ID + Order Number is required."),
                StepRunErrorCategory.Validation);
        }

        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException("Order not found."),
                StepRunErrorCategory.Validation);
        }

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(order.StoreId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        var orderJson = new JsonObject
        {
            ["id"] = order.Id.ToString(),
            ["orderNumber"] = order.OrderNumber,
            ["storeId"] = order.StoreId.ToString(),
            ["customerEmail"] = order.CustomerInfo?.Email,
            ["customerFirstName"] = order.CustomerInfo?.FirstName,
            ["customerLastName"] = order.CustomerInfo?.LastName,
            ["customerReference"] = order.CustomerInfo?.CustomerReference,
            ["currencyId"] = order.CurrencyId.ToString(),
            ["isFinalized"] = order.IsFinalized,
            ["paymentStatus"] = order.TransactionInfo?.PaymentStatus?.ToString(),
            ["orderStatusId"] = order.OrderStatusId.ToString(),
        };

        return Success(new GetOrderOutput
        {
            OrderId = order.Id,
            StoreId = order.StoreId,
            OrderNumber = order.OrderNumber,
            CustomerEmail = order.CustomerInfo?.Email,
            CustomerFirstName = order.CustomerInfo?.FirstName,
            CustomerLastName = order.CustomerInfo?.LastName,
            CustomerReference = order.CustomerInfo?.CustomerReference,
            CurrencyId = order.CurrencyId,
            IsFinalized = order.IsFinalized,
            PaymentStatus = order.TransactionInfo?.PaymentStatus?.ToString(),
            OrderStatusId = order.OrderStatusId,
            OrderJson = orderJson.ToJsonString(),
        });
    }
}
