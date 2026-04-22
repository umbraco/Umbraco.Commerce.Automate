using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="RemoveOrderLineAction"/>.
/// </summary>
public sealed class RemoveOrderLineSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to update.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the order line ID to remove.
    /// </summary>
    [Field(Label = "Order Line ID", Description = "The ID of the order line to remove.", SortOrder = 1, SupportsBindings = true)]
    public string OrderLineId { get; set; } = string.Empty;
}
