namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Output produced by the <see cref="PaymentUpdatedTrigger"/>.
/// </summary>
public sealed class PaymentUpdatedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public string? PaymentStatusFrom { get; init; }
    public string? PaymentStatusTo { get; init; }
    public string? TransactionId { get; init; }
    public decimal AmountAuthorized { get; init; }
    public decimal TransactionFee { get; init; }
}
