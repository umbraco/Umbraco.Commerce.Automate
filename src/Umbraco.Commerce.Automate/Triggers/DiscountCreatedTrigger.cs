using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.discountCreated", "Discount Created",
    Description = "Fires when a discount is created.",
    Group = "Commerce",
    Icon = "icon-discount")]
public sealed class DiscountCreatedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, DiscountCreatedTriggerOutput, CommerceDiscountCreatedNotification>
{
    public DiscountCreatedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceDiscountCreatedNotification notification)
    {
        var discount = notification.Discount;
        yield return new TriggerEvent<DiscountCreatedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(discount.Id, discount.Id.GetHashCode()),
            Output = new DiscountCreatedTriggerOutput
            {
                DiscountId = discount.Id,
                StoreId = discount.StoreId,
                Name = discount.Name,
            },
        };
    }
}
