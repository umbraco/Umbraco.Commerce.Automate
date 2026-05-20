using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Finalizes a Commerce order manually (for offline/admin/bank-transfer payments).
/// </summary>
[Action("umbracoCommerce.finalizeOrder", "Finalize Order",
    Description = "Finalizes a Commerce order manually (for offline/admin/bank-transfer payments).",
    Group = "Commerce",
    Icon = "icon-receipt-dollar",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class FinalizeOrderAction : ActionBase<FinalizeOrderSettings, FinalizeOrderOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public FinalizeOrderAction(
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
        var settings = context.GetSettings<FinalizeOrderSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (!Enum.TryParse<PaymentStatus>(settings.PaymentStatus, ignoreCase: true, out var paymentStatus))
        {
            return ActionResult.Failed(
                new ArgumentException($"'{settings.PaymentStatus}' is not a valid Payment Status."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        if (await _storeAuthorizer.AuthorizeStoreOrFailAsync(order.StoreId, cancellationToken) is { } storeAuthFailure)
        {
            return storeAuthFailure;
        }

        if (order.IsFinalized)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' is already finalized."),
                StepRunErrorCategory.Validation);
        }

        await _uowProvider.ExecuteAsync(async uow =>
        {
            var writableOrder = await order.AsWritableAsync(uow);
            await writableOrder.FinalizeAsync(
                settings.AmountAuthorized,
                settings.TransactionFee,
                settings.TransactionId ?? string.Empty,
                paymentStatus);
            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new FinalizeOrderOutput
        {
            OrderId = orderId,
            OrderNumber = order.OrderNumber,
            PaymentStatus = paymentStatus.ToString(),
        });
    }
}
