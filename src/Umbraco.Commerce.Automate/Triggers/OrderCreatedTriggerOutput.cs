using Umbraco.Commerce.Automate.Dispatch;

namespace Umbraco.Commerce.Automate.Triggers;

public sealed class OrderCreatedTriggerOutput : IStoreScopedTriggerOutput
{
    public Guid OrderId { get; init; }
    public Guid StoreId { get; init; }
    public Guid CurrencyId { get; init; }

    Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;
}
