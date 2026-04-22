namespace Umbraco.Commerce.Automate.Triggers;

public sealed class DiscountCreatedTriggerOutput
{
    public Guid DiscountId { get; init; }
    public Guid StoreId { get; init; }
    public string? Name { get; init; }
}
