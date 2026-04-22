using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class GiftCardCreatedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<GiftCardCreatedNotification>
{
    public override async Task HandleAsync(GiftCardCreatedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceGiftCardCreatedNotification(evt.GiftCard), cancellationToken);
    }
}
