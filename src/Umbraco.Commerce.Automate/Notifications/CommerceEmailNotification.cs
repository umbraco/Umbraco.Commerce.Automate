using Umbraco.Cms.Core.Notifications;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's EmailSentNotification and EmailFailedNotification.
/// </summary>
public sealed class CommerceEmailNotification(
    string toEmailAddress,
    string? emailTemplateAlias,
    bool success) : INotification
{
    public string ToEmailAddress { get; } = toEmailAddress;
    public string? EmailTemplateAlias { get; } = emailTemplateAlias;
    public bool Success { get; } = success;
}
