namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="CapturePaymentAction"/>.
/// </summary>
public sealed class CapturePaymentOutput
{
    public Guid OrderId { get; init; }
    public bool Success { get; init; }
    public string? PaymentStatus { get; init; }
    public string? TransactionId { get; init; }
}
