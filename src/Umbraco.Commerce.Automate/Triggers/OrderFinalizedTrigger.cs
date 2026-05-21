using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Fires when an order is finalized in Umbraco Commerce.
/// </summary>
[Trigger("umbracoCommerce.orderFinalized", "Order Finalized",
    Description = "Fires when an order is finalized.",
    Group = "Commerce",
    Icon = "icon-receipt-dollar",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class OrderFinalizedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, OrderFinalizedTriggerOutput, CommerceOrderFinalizedNotification>
{
    public OrderFinalizedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderFinalizedNotification notification)
    {
        var order = notification.Order;

        yield return new TriggerEvent<OrderFinalizedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new OrderFinalizedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                CustomerEmail = order.CustomerInfo?.Email,
                CustomerFirstName = order.CustomerInfo?.FirstName,
                CustomerLastName = order.CustomerInfo?.LastName,
                CurrencyId = order.CurrencyId,
                PaymentStatus = order.TransactionInfo?.PaymentStatus?.ToString(),
            },
        };
    }
}
