using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Notifications.Handlers;

internal sealed class EmailSentBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<EmailSentNotification>
{
    public override async Task HandleAsync(EmailSentNotification evt, CancellationToken cancellationToken)
    {
        await eventAggregator.PublishAsync(
            new CommerceEmailNotification(evt.EmailContext.ToEmailAddress, evt.EmailContext.EmailTemplate?.Alias, success: true),
            cancellationToken);
    }
}
