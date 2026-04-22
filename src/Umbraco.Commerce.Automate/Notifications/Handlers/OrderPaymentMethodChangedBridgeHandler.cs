using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class OrderPaymentMethodChangedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderPaymentMethodChangedNotification>
{
    public override async Task HandleAsync(OrderPaymentMethodChangedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceOrderPaymentMethodChangedNotification(evt.Order, evt.PaymentMethodId),
            cancellationToken);
    }
}
