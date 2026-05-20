using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.emailFailed", "Email Failed",
    Description = "Fires when a Commerce email fails to send.",
    Group = "Commerce",
    Icon = "icon-alert",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class EmailFailedTrigger
    : NotificationTriggerBase<object, EmailEventTriggerOutput, CommerceEmailNotification>
{
    public EmailFailedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceEmailNotification notification)
    {
        if (notification.Success) yield break;

        yield return new TriggerEvent<EmailEventTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            Output = new EmailEventTriggerOutput
            {
                ToEmailAddress = notification.ToEmailAddress,
                EmailTemplateAlias = notification.EmailTemplateAlias,
                Success = false,
            },
        };
    }
}
