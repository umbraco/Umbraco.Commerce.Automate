using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderStatusChangedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderStatusChangedNotification>
{
    public override async Task HandleAsync(OrderStatusChangedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderStatusChangedNotification(evt.Order, evt.OrderStatusId, evt.OrderStatusCode),
            cancellationToken);
    }
}
