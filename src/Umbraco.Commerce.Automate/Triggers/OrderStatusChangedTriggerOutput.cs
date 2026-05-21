using Umbraco.Commerce.Automate.Dispatch;

namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Output produced by the <see cref="OrderStatusChangedTrigger"/>.
/// </summary>
public sealed class OrderStatusChangedTriggerOutput : IStoreScopedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public Guid? PreviousStatusId { get; init; }
    public Guid? NewStatusId { get; init; }

    Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;
}
