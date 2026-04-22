using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class PaymentUpdatedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderTransactionUpdatedNotification>
{
    public override async Task HandleAsync(OrderTransactionUpdatedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommercePaymentUpdatedNotification(
                evt.Order, evt.PaymentStatus, evt.TransactionId, evt.AmountAuthorized, evt.TransactionFee),
            cancellationToken);
    }
}
