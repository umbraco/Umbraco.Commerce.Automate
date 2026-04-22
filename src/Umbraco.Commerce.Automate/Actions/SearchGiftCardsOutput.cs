namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="SearchGiftCardsAction"/>.
/// </summary>
public sealed class SearchGiftCardsOutput
{
    public long TotalCount { get; init; }
    public int ReturnedCount { get; init; }
    public string? GiftCardsJson { get; init; }
}
