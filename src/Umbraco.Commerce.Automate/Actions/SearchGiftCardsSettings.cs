using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="SearchGiftCardsAction"/>.
/// </summary>
public sealed class SearchGiftCardsSettings
{
    /// <summary>
    /// Gets or sets the store ID to search within.
    /// </summary>
    [Field(Label = "Store ID", Description = "The store to search gift cards in.", SupportsBindings = true)]
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional search term for gift card fields.
    /// </summary>
    [Field(Label = "Search Term", Description = "Search gift card code or searchable fields.", SortOrder = 1, SupportsBindings = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of gift cards to return.
    /// </summary>
    [Field(Label = "Page Size", Description = "Maximum number of gift cards to return (default: 50).", SortOrder = 2)]
    public int PageSize { get; set; } = 50;
}
