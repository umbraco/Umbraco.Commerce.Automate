using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="SearchOrdersAction"/>.
/// </summary>
public sealed class SearchOrdersSettings
{
    /// <summary>
    /// Gets or sets the store ID to search within.
    /// </summary>
    [Field(Label = "Store ID", Description = "The store to search orders in.", SupportsBindings = true)]
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional search term for order fields.
    /// </summary>
    [Field(Label = "Search Term", Description = "Search order number, customer name, or email.", SortOrder = 1, SupportsBindings = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of orders to return.
    /// </summary>
    [Field(Label = "Page Size", Description = "Maximum number of orders to return (default: 50).", SortOrder = 2)]
    public int PageSize { get; set; } = 50;
}
