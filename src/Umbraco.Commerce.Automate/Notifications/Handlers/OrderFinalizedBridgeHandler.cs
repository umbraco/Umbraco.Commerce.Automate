using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderFinalizedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderFinalizedNotification>
{
    public override async Task HandleAsync(OrderFinalizedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderFinalizedNotification(evt.Order), cancellationToken);
    }
}
