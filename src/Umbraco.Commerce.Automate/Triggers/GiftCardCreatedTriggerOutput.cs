using Umbraco.Commerce.Automate.Dispatch;

namespace Umbraco.Commerce.Automate.Triggers;

public sealed class GiftCardCreatedTriggerOutput : IStoreScopedTriggerOutput
{
    public Guid GiftCardId { get; init; }
    public Guid StoreId { get; init; }
    public string? Code { get; init; }

    Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;
}
