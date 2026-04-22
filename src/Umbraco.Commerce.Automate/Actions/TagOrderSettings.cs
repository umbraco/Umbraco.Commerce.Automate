using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="TagOrderAction"/>.
/// </summary>
public sealed class TagOrderSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to tag.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags to add (comma-separated).
    /// </summary>
    [Field(Label = "Tags", Description = "Comma-separated tags to add to the order.", SortOrder = 1, SupportsBindings = true)]
    public string Tags { get; set; } = string.Empty;
}
