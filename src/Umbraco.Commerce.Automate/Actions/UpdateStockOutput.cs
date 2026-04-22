namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="UpdateStockAction"/>.
/// </summary>
public sealed class UpdateStockOutput
{
    public Guid StoreId { get; init; }
    public string? ProductReference { get; init; }
    public string? ProductVariantReference { get; init; }
    public string? Operation { get; init; }
    public decimal Value { get; init; }
    public decimal? NewStockLevel { get; init; }
}
