using Umbraco.Commerce.Automate.Dispatch;

namespace Umbraco.Commerce.Automate.Triggers;

public sealed class CodeRedeemedTriggerOutput : IStoreScopedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public string? Code { get; init; }
    public string? CodeType { get; init; }

    Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;
}
