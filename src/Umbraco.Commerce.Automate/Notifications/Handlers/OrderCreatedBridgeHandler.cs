using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderCreatedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderCreatedNotification>
{
    public override async Task HandleAsync(OrderCreatedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderCreatedNotification(evt.Order), cancellationToken);
    }
}
