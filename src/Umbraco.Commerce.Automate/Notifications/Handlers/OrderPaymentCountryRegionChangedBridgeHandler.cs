using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderPaymentCountryRegionChangedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderPaymentCountryRegionChangedNotification>
{
    public override async Task HandleAsync(OrderPaymentCountryRegionChangedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderCountryRegionChangedNotification(evt.Order, evt.CountryId, evt.RegionId, "Payment"),
            cancellationToken);
    }
}
