using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class EmailFailedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<EmailFailedNotification>
{
    public override async Task HandleAsync(EmailFailedNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceEmailNotification(evt.EmailContext.ToEmailAddress, evt.EmailContext.EmailTemplate?.Alias, success: false),
            cancellationToken);
    }
}
