using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.paymentMethodChanged", "Payment Method Changed",
    Description = "Fires when an order's payment method changes.",
    Group = "Commerce",
    Icon = "icon-credit-card",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class PaymentMethodChangedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, PaymentMethodChangedTriggerOutput, CommerceOrderPaymentMethodChangedNotification>
{
    public PaymentMethodChangedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderPaymentMethodChangedNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<PaymentMethodChangedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new PaymentMethodChangedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                PreviousPaymentMethodId = notification.PaymentMethodId.From,
                NewPaymentMethodId = notification.PaymentMethodId.To,
            },
        };
    }
}
