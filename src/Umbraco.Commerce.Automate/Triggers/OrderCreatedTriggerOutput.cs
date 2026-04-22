namespace Umbraco.Commerce.Automate.Triggers;

public sealed class OrderCreatedTriggerOutput
{
    public Guid OrderId { get; init; }
    public Guid StoreId { get; init; }
    public Guid CurrencyId { get; init; }
}
