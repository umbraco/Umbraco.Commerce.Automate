using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="SetOrderPropertyAction"/>.
/// </summary>
public sealed class SetOrderPropertySettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to update.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the property alias.
    /// </summary>
    [Field(Label = "Property Alias", Description = "The alias of the property to set.", SortOrder = 1, SupportsBindings = true)]
    public string Alias { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the property value.
    /// </summary>
    [Field(Label = "Value", Description = "The value to set for the property.", SortOrder = 2, SupportsBindings = true)]
    public string? Value { get; set; }
}
