using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderProductAddedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderProductAddedNotification>
{
    public override async Task HandleAsync(OrderProductAddedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderProductAddedNotification(evt.Order, evt.ProductReference, evt.ProductVariantReference, evt.Quantity),
            cancellationToken);
    }
}
