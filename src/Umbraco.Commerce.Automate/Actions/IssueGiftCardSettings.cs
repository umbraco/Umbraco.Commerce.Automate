using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="IssueGiftCardAction"/>.
/// </summary>
public sealed class IssueGiftCardSettings
{
    /// <summary>
    /// Gets or sets the store ID.
    /// </summary>
    [Field(Label = "Store ID", Description = "The store to issue the gift card in.", SupportsBindings = true)]
    public string StoreId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the currency ID.
    /// </summary>
    [Field(Label = "Currency ID", Description = "The currency the gift card is issued in.", SortOrder = 1, SupportsBindings = true)]
    public string CurrencyId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the original amount of the gift card.
    /// </summary>
    [Field(Label = "Amount", Description = "The value of the gift card.", SortOrder = 2, SupportsBindings = true)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the gift card code. When empty a code is auto-generated.
    /// </summary>
    [Field(Label = "Code", Description = "Optional gift card code. When empty a code is generated automatically.", SortOrder = 3, SupportsBindings = true)]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets an optional expiry date (ISO-8601).
    /// </summary>
    [Field(Label = "Expiry Date", Description = "Optional expiry date in ISO-8601 format (e.g. 2026-12-31).", SortOrder = 4, SupportsBindings = true)]
    public string? ExpiryDate { get; set; }
}
