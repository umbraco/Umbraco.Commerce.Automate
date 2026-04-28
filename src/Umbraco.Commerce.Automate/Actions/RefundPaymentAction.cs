using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Refunds the payment on a Commerce order via the payment provider.
/// </summary>
[Action("umbracoCommerce.refundPayment", "Refund Payment",
    Description = "Refunds the payment on a Commerce order via the payment provider.",
    Group = "Commerce",
    Icon = "icon-undo")]
public sealed class RefundPaymentAction : ActionBase<RefundPaymentSettings, RefundPaymentOutput>
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWorkProvider _uowProvider;

    public RefundPaymentAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IPaymentService paymentService,
        IUnitOfWorkProvider uowProvider)
        : base(infrastructure)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _uowProvider = uowProvider;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<RefundPaymentSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        var refundRequest = new OrderRefundRequest
        {
            RefundAmount = settings.RefundAmount,
            RestockProducts = settings.RestockProducts,
        };

        var result = await _paymentService.RefundOrderPaymentAsync(order, refundRequest, cancellationToken);

        if (result.Success)
        {
            await _uowProvider.ExecuteAsync(async uow =>
            {
                var writableOrder = await order.AsWritableAsync(uow);
                await writableOrder.ApplyPaymentChangesAsync(result);
                await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
                uow.Complete();
            }, cancellationToken);
        }

        return Success(new RefundPaymentOutput
        {
            OrderId = orderId,
            Success = result.Success,
            PaymentStatus = result.TransactionInfo?.PaymentStatus.ToString(),
            TransactionId = result.TransactionInfo?.TransactionId,
        });
    }
}
