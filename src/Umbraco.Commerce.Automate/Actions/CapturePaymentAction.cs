using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Captures an authorized payment on a Commerce order via the payment provider.
/// </summary>
[Action("umbracoCommerce.capturePayment", "Capture Payment",
    Description = "Captures an authorized payment on a Commerce order via the payment provider.",
    Group = "Commerce",
    Icon = "icon-coins-alt",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class CapturePaymentAction : ActionBase<CapturePaymentSettings, CapturePaymentOutput>
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWorkProvider _uowProvider;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public CapturePaymentAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IPaymentService paymentService,
        IUnitOfWorkProvider uowProvider,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _uowProvider = uowProvider;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<CapturePaymentSettings>();

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

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(order.StoreId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        var result = await _paymentService.CaptureOrderPaymentAsync(order, cancellationToken);

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

        return Success(new CapturePaymentOutput
        {
            OrderId = orderId,
            Success = result.Success,
            PaymentStatus = result.TransactionInfo?.PaymentStatus.ToString(),
            TransactionId = result.TransactionInfo?.TransactionId,
        });
    }
}
