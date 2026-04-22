using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderDiscountCodeRedeemedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderDiscountCodeRedeemedNotification>
{
    public override async Task HandleAsync(OrderDiscountCodeRedeemedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderCodeRedeemedNotification(evt.Order, evt.Code, "Discount"),
            cancellationToken);
    }
}
