namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="GetOrderAction"/>.
/// </summary>
public sealed class GetOrderOutput
{
    public Guid OrderId { get; init; }
    public Guid StoreId { get; init; }
    public string? OrderNumber { get; init; }
    public string? CustomerEmail { get; init; }
    public string? CustomerFirstName { get; init; }
    public string? CustomerLastName { get; init; }
    public string? CustomerReference { get; init; }
    public Guid CurrencyId { get; init; }
    public bool IsFinalized { get; init; }
    public string? PaymentStatus { get; init; }
    public Guid OrderStatusId { get; init; }
    public string? OrderJson { get; init; }
}
