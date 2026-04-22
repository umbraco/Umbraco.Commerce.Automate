namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Output produced by the <see cref="OrderFinalizedTrigger"/>.
/// </summary>
public sealed class OrderFinalizedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public string? CustomerEmail { get; init; }
    public string? CustomerFirstName { get; init; }
    public string? CustomerLastName { get; init; }
    public Guid CurrencyId { get; init; }
    public string? PaymentStatus { get; init; }
}
