using Umbraco.Commerce.Automate.Dispatch;

namespace Umbraco.Commerce.Automate.Triggers;

public sealed class PaymentMethodChangedTriggerOutput : IStoreScopedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public Guid? PreviousPaymentMethodId { get; init; }
    public Guid? NewPaymentMethodId { get; init; }

    Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;
}
