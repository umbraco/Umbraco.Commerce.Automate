using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderShippingMethodChangedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderShippingMethodChangedNotification>
{
    public override async Task HandleAsync(OrderShippingMethodChangedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderShippingMethodChangedNotification(evt.Order, evt.ShippingMethodId),
            cancellationToken);
    }
}
