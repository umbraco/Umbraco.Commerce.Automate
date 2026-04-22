using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class DiscountCreatedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<DiscountCreatedNotification>
{
    public override async Task HandleAsync(DiscountCreatedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceDiscountCreatedNotification(evt.Discount), cancellationToken);
    }
}
