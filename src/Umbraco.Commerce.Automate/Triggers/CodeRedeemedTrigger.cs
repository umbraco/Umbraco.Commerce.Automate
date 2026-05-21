using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.codeRedeemed", "Discount/Gift Card Code Redeemed",
    Description = "Fires when a discount code or gift card is redeemed on an order.",
    Group = "Commerce",
    Icon = "icon-tag",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class CodeRedeemedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, CodeRedeemedTriggerOutput, CommerceOrderCodeRedeemedNotification>
{
    public CodeRedeemedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderCodeRedeemedNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<CodeRedeemedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new CodeRedeemedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                Code = notification.Code,
                CodeType = notification.CodeType,
            },
        };
    }
}
