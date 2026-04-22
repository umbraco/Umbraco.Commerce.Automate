using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Removes an order line from a Commerce order.
/// </summary>
[Action("umbracoCommerce.removeOrderLine", "Remove Order Line",
    Description = "Removes an order line from a Commerce order.",
    Group = "Commerce",
    Icon = "icon-remove")]
public sealed class RemoveOrderLineAction : ActionBase<RemoveOrderLineSettings, RemoveOrderLineOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;

    public RemoveOrderLineAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IUnitOfWorkProvider uowProvider)
        : base(infrastructure)
    {
        _orderService = orderService;
        _uowProvider = uowProvider;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<RemoveOrderLineSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (!Guid.TryParse(settings.OrderLineId, out var orderLineId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order Line ID is required."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        await _uowProvider.ExecuteAsync(async uow =>
        {
            var writableOrder = await order.AsWritableAsync(uow);
            await writableOrder.RemoveOrderLineAsync(orderLineId);
            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new RemoveOrderLineOutput
        {
            OrderId = orderId,
            OrderLineId = orderLineId,
        });
    }
}
