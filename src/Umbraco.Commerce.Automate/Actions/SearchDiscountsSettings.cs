using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="SearchDiscountsAction"/>.
/// </summary>
public sealed class SearchDiscountsSettings
{
    /// <summary>
    /// Gets or sets the store ID to search within.
    /// </summary>
    [Field(Label = "Store ID", Description = "The store to search discounts in.", SupportsBindings = true)]
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional search term that filters discounts by name or alias.
    /// </summary>
    [Field(Label = "Search Term", Description = "Optional filter on discount name or alias (case-insensitive).", SortOrder = 1, SupportsBindings = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// When true, returns only currently-active discounts.
    /// </summary>
    [Field(Label = "Only Active", Description = "When true, returns only discounts currently active.", SortOrder = 2)]
    public bool OnlyActive { get; set; }
}
