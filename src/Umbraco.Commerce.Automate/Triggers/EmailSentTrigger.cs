using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.emailSent", "Email Sent",
    Description = "Fires when a Commerce email is sent successfully.",
    Group = "Commerce",
    Icon = "icon-message")]
public sealed class EmailSentTrigger
    : NotificationTriggerBase<object, EmailEventTriggerOutput, CommerceEmailNotification>
{
    public EmailSentTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceEmailNotification notification)
    {
        if (!notification.Success) yield break;

        yield return new TriggerEvent<EmailEventTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            Output = new EmailEventTriggerOutput
            {
                ToEmailAddress = notification.ToEmailAddress,
                EmailTemplateAlias = notification.EmailTemplateAlias,
                Success = true,
            },
        };
    }
}
