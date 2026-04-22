using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderShippingCountryRegionChangedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderShippingCountryRegionChangedNotification>
{
    public override async Task HandleAsync(OrderShippingCountryRegionChangedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderCountryRegionChangedNotification(evt.Order, evt.CountryId, evt.RegionId, "Shipping"),
            cancellationToken);
    }
}
