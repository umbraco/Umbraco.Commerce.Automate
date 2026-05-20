using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.orderCreated", "Order Created",
    Description = "Fires when a new order is created.",
    Group = "Commerce",
    Icon = "icon-add",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class OrderCreatedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, OrderCreatedTriggerOutput, CommerceOrderCreatedNotification>
{
    public OrderCreatedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderCreatedNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<OrderCreatedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new OrderCreatedTriggerOutput
            {
                OrderId = order.Id,
                StoreId = order.StoreId,
                CurrencyId = order.CurrencyId,
            },
        };
    }
}
