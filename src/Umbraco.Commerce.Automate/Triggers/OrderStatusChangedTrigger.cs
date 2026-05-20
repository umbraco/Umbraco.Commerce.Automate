using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Fires when an order's status changes in Umbraco Commerce.
/// </summary>
[Trigger("umbracoCommerce.orderStatusChanged", "Order Status Changed",
    Description = "Fires when an order's status changes.",
    Group = "Commerce",
    Icon = "icon-arrow-right",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class OrderStatusChangedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, OrderStatusChangedTriggerOutput, CommerceOrderStatusChangedNotification>
{
    public OrderStatusChangedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderStatusChangedNotification notification)
    {
        var order = notification.Order;

        yield return new TriggerEvent<OrderStatusChangedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new OrderStatusChangedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                PreviousStatusId = notification.OrderStatusId.From,
                NewStatusId = notification.OrderStatusId.To,
            },
        };
    }
}
