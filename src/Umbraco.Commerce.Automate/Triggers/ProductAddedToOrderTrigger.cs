using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.productAddedToOrder", "Product Added to Order",
    Description = "Fires when a product is added to an order.",
    Group = "Commerce",
    Icon = "icon-shopping-basket-alt-2")]
public sealed class ProductAddedToOrderTrigger
    : NotificationTriggerBase<StoreTriggerSettings, ProductAddedToOrderTriggerOutput, CommerceOrderProductAddedNotification>
{
    public ProductAddedToOrderTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderProductAddedNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<ProductAddedToOrderTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new ProductAddedToOrderTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                ProductReference = notification.ProductReference,
                ProductVariantReference = notification.ProductVariantReference,
                Quantity = notification.Quantity,
            },
        };
    }
}
