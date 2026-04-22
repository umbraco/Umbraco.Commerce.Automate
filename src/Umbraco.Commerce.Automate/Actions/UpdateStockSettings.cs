using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="UpdateStockAction"/>.
/// </summary>
public sealed class UpdateStockSettings
{
    /// <summary>
    /// Gets or sets the store ID.
    /// </summary>
    [Field(Label = "Store ID", Description = "The store the product belongs to.", SupportsBindings = true)]
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the product reference.
    /// </summary>
    [Field(Label = "Product Reference", Description = "Unique reference of the product.", SortOrder = 1, SupportsBindings = true)]
    public string ProductReference { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the optional product variant reference.
    /// </summary>
    [Field(Label = "Variant Reference", Description = "Optional variant reference.", SortOrder = 2, SupportsBindings = true)]
    public string? ProductVariantReference { get; set; }

    /// <summary>
    /// Gets or sets the operation: Set, Increase, or Decrease.
    /// </summary>
    [Field(Label = "Operation", Description = "Operation to perform: Set, Increase, or Decrease.", SortOrder = 3, SupportsBindings = true)]
    public string Operation { get; set; } = "Set";

    /// <summary>
    /// Gets or sets the stock value or delta.
    /// </summary>
    [Field(Label = "Value", Description = "The stock level (for Set) or the amount to increase/decrease by.", SortOrder = 4, SupportsBindings = true)]
    public decimal Value { get; set; }
}
