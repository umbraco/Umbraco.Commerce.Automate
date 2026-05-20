using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Fires when an order's payment transaction is updated in Umbraco Commerce.
/// </summary>
[Trigger("umbracoCommerce.paymentUpdated", "Payment Updated",
    Description = "Fires when an order's payment transaction is updated.",
    Group = "Commerce",
    Icon = "icon-coins-alt",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class PaymentUpdatedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, PaymentUpdatedTriggerOutput, CommercePaymentUpdatedNotification>
{
    public PaymentUpdatedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<TriggerEvent> MapEvent(CommercePaymentUpdatedNotification notification)
    {
        var order = notification.Order;

        yield return new TriggerEvent<PaymentUpdatedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new PaymentUpdatedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                PaymentStatusFrom = notification.PaymentStatus.From?.ToString(),
                PaymentStatusTo = notification.PaymentStatus.To?.ToString(),
                TransactionId = notification.TransactionId.To,
                AmountAuthorized = notification.AmountAuthorized.To,
                TransactionFee = notification.TransactionFee.To,
            },
        };
    }
}
