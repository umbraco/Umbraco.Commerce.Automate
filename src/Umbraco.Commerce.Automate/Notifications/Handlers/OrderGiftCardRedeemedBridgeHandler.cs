using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderGiftCardRedeemedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderGiftCardRedeemedNotification>
{
    public override async Task HandleAsync(OrderGiftCardRedeemedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderCodeRedeemedNotification(evt.Order, evt.Code, "GiftCard"),
            cancellationToken);
    }
}
