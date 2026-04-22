using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="SendOrderEmailAction"/>.
/// </summary>
public sealed class SendOrderEmailSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to send the email for.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email template alias.
    /// </summary>
    [Field(Label = "Email Template Alias", Description = "The alias of the email template to send.", SortOrder = 1, SupportsBindings = true)]
    public string EmailTemplateAlias { get; set; } = string.Empty;
}
