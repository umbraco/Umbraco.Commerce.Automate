using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.shippingMethodChanged", "Shipping Method Changed",
    Description = "Fires when an order's shipping method changes.",
    Group = "Commerce",
    Icon = "icon-truck")]
public sealed class ShippingMethodChangedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, ShippingMethodChangedTriggerOutput, CommerceOrderShippingMethodChangedNotification>
{
    public ShippingMethodChangedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderShippingMethodChangedNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<ShippingMethodChangedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new ShippingMethodChangedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                PreviousShippingMethodId = notification.ShippingMethodId.From,
                NewShippingMethodId = notification.ShippingMethodId.To,
            },
        };
    }
}
