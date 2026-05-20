using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Fires when product stock changes in Umbraco Commerce.
/// </summary>
[Trigger("umbracoCommerce.stockChanged", "Stock Changed",
    Description = "Fires when product stock changes.",
    Group = "Commerce",
    Icon = "icon-box",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class StockChangedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, StockChangedTriggerOutput, CommerceStockChangedNotification>
{
    public StockChangedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure)
    {
    }

    /// <inheritdoc />
    public override IEnumerable<TriggerEvent> MapEvent(CommerceStockChangedNotification notification)
    {
        yield return new TriggerEvent<StockChangedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = $"{Alias}:{notification.ProductReference}:{notification.ProductVariantReference}:{DateTime.UtcNow:O}",
            Output = new StockChangedTriggerOutput
            {
                StoreId = notification.StoreId,
                ProductReference = notification.ProductReference,
                ProductVariantReference = notification.ProductVariantReference,
                PreviousStock = notification.Value.From,
                NewStock = notification.Value.To,
            },
        };
    }
}
