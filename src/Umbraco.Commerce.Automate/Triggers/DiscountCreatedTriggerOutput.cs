using Umbraco.Commerce.Automate.Dispatch;

namespace Umbraco.Commerce.Automate.Triggers;

public sealed class DiscountCreatedTriggerOutput : IStoreScopedTriggerOutput
{
    public Guid DiscountId { get; init; }
    public Guid StoreId { get; init; }
    public string? Name { get; init; }

    Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;
}
