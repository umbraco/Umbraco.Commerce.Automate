namespace Umbraco.Commerce.Automate.Triggers;

public sealed class ProductAddedToOrderTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public string? ProductReference { get; init; }
    public string? ProductVariantReference { get; init; }
    public decimal Quantity { get; init; }
}
