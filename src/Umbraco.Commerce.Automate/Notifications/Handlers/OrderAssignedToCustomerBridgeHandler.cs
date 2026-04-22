using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderAssignedToCustomerBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderAssignedToCustomerNotification>
{
    public override async Task HandleAsync(OrderAssignedToCustomerNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderAssignedToCustomerNotification(evt.Order, evt.CustomerReference),
            cancellationToken);
    }
}
