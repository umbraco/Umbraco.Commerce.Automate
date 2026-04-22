using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Assigns a Commerce order to a customer reference.
/// </summary>
[Action("umbracoCommerce.assignOrderToCustomer", "Assign Order to Customer",
    Description = "Assigns a Commerce order to a known customer reference.",
    Group = "Commerce",
    Icon = "icon-user")]
public sealed class AssignOrderToCustomerAction : ActionBase<AssignOrderToCustomerSettings, AssignOrderToCustomerOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;

    public AssignOrderToCustomerAction(
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
        var settings = context.GetSettings<AssignOrderToCustomerSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (string.IsNullOrWhiteSpace(settings.CustomerReference))
        {
            return ActionResult.Failed(
                new ArgumentException("A Customer Reference is required."),
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
            await writableOrder.AssignToCustomerAsync(settings.CustomerReference);
            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new AssignOrderToCustomerOutput
        {
            OrderId = orderId,
            CustomerReference = settings.CustomerReference,
        });
    }
}
