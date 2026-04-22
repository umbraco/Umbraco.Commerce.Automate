using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.orderAssignedToCustomer", "Order Assigned to Customer",
    Description = "Fires when an order is assigned to a customer.",
    Group = "Commerce",
    Icon = "icon-user")]
public sealed class OrderAssignedToCustomerTrigger
    : NotificationTriggerBase<StoreTriggerSettings, OrderAssignedToCustomerTriggerOutput, CommerceOrderAssignedToCustomerNotification>
{
    public OrderAssignedToCustomerTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderAssignedToCustomerNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<OrderAssignedToCustomerTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new OrderAssignedToCustomerTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                PreviousCustomerReference = notification.CustomerReference.From,
                NewCustomerReference = notification.CustomerReference.To,
            },
        };
    }
}
