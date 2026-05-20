using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Changes the status of a Commerce order.
/// </summary>
[Action("umbracoCommerce.changeOrderStatus", "Change Order Status",
    Description = "Changes the status of a Commerce order.",
    Group = "Commerce",
    Icon = "icon-arrow-right",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class ChangeOrderStatusAction : ActionBase<ChangeOrderStatusSettings, ChangeOrderStatusOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public ChangeOrderStatusAction(
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
        var settings = context.GetSettings<ChangeOrderStatusSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (!Guid.TryParse(settings.OrderStatusId, out var statusId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order Status ID is required."),
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
            await writableOrder.SetOrderStatusAsync(statusId);
            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new ChangeOrderStatusOutput
        {
            OrderId = orderId,
            OrderStatusId = statusId,
        });
    }
}
