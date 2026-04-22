using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="RemoveTagsFromOrderAction"/>.
/// </summary>
public sealed class RemoveTagsFromOrderSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to update.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tags to remove (comma-separated).
    /// </summary>
    [Field(Label = "Tags", Description = "Comma-separated list of tags to remove from the order.", SortOrder = 1, SupportsBindings = true)]
    public string Tags { get; set; } = string.Empty;
}
