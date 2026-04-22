namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="AddProductToOrderAction"/>.
/// </summary>
public sealed class AddProductToOrderOutput
{
    public Guid OrderId { get; init; }
    public string? ProductReference { get; init; }
    public string? ProductVariantReference { get; init; }
    public decimal Quantity { get; init; }
}
