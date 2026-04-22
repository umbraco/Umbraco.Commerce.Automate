namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="IssueGiftCardAction"/>.
/// </summary>
public sealed class IssueGiftCardOutput
{
    public Guid GiftCardId { get; init; }
    public string? Code { get; init; }
    public decimal Amount { get; init; }
    public Guid CurrencyId { get; init; }
    public Guid StoreId { get; init; }
    public DateTime? ExpiryDate { get; init; }
}
