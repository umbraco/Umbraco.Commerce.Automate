using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="AddProductToOrderAction"/>.
/// </summary>
public sealed class AddProductToOrderSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to add the product to.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product reference.
    /// </summary>
    [Field(Label = "Product Reference", Description = "Unique reference of the product to add.", SortOrder = 1, SupportsBindings = true)]
    public string ProductReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional variant reference.
    /// </summary>
    [Field(Label = "Variant Reference", Description = "Optional variant reference.", SortOrder = 2, SupportsBindings = true)]
    public string? ProductVariantReference { get; set; }

    /// <summary>
    /// Gets or sets the quantity to add.
    /// </summary>
    [Field(Label = "Quantity", Description = "Quantity to add.", SortOrder = 3, SupportsBindings = true)]
    public decimal Quantity { get; set; } = 1;
}
