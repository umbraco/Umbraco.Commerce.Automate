namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="FinalizeOrderAction"/>.
/// </summary>
public sealed class FinalizeOrderOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public string? PaymentStatus { get; init; }
}
