namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="RefundPaymentAction"/>.
/// </summary>
public sealed class RefundPaymentOutput
{
    public Guid OrderId { get; init; }
    public bool Success { get; init; }
    public string? PaymentStatus { get; init; }
    public string? TransactionId { get; init; }
}
