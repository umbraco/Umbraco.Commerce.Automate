using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="GetOrderAction"/>.
/// </summary>
public sealed class GetOrderSettings
{
    /// <summary>
    /// Gets or sets the order ID. Takes precedence over <see cref="OrderNumber"/> when provided.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order GUID to fetch. Takes precedence over Order Number if both are provided.", SupportsBindings = true)]
    public string? OrderId { get; set; }

    /// <summary>
    /// Gets or sets the store ID. Required when looking up by order number.
    /// </summary>
    [Field(Label = "Store ID", Description = "The store the order belongs to. Required when looking up by Order Number.", SortOrder = 1, SupportsBindings = true)]
    public string? StoreId { get; set; }

    /// <summary>
    /// Gets or sets the cart or order number.
    /// </summary>
    [Field(Label = "Order Number", Description = "The cart or order number. Requires a Store ID.", SortOrder = 2, SupportsBindings = true)]
    public string? OrderNumber { get; set; }
}
