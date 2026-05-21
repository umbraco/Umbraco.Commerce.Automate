using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.giftCardCreated", "Gift Card Created",
    Description = "Fires when a gift card is created.",
    Group = "Commerce",
    Icon = "icon-gift",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class GiftCardCreatedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, GiftCardCreatedTriggerOutput, CommerceGiftCardCreatedNotification>
{
    public GiftCardCreatedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceGiftCardCreatedNotification notification)
    {
        var gc = notification.GiftCard;
        yield return new TriggerEvent<GiftCardCreatedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(gc.Id, gc.CreateDate.GetHashCode()),
            Output = new GiftCardCreatedTriggerOutput
            {
                GiftCardId = gc.Id,
                StoreId = gc.StoreId,
                Code = gc.Code,
            },
        };
    }
}
