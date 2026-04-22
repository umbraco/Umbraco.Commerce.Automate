using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class StockChangedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<StockChangedNotification>
{
    public override async Task HandleAsync(StockChangedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceStockChangedNotification(evt.StoreId, evt.ProductReference, evt.ProductVariantReference, evt.Value),
            cancellationToken);
    }
}
