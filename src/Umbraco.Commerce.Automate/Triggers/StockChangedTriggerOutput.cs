namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Output produced by the <see cref="StockChangedTrigger"/>.
/// </summary>
public sealed class StockChangedTriggerOutput
{
    public Guid StoreId { get; init; }
    public string? ProductReference { get; init; }
    public string? ProductVariantReference { get; init; }
    public decimal? PreviousStock { get; init; }
    public decimal? NewStock { get; init; }
}
