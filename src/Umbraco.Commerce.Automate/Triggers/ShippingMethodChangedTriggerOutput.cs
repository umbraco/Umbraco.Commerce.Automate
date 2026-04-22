namespace Umbraco.Commerce.Automate.Triggers;

public sealed class ShippingMethodChangedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public Guid? PreviousShippingMethodId { get; init; }
    public Guid? NewShippingMethodId { get; init; }
}
