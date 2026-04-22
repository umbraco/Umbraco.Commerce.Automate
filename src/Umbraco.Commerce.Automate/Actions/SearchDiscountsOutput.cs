namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="SearchDiscountsAction"/>.
/// </summary>
public sealed class SearchDiscountsOutput
{
    public int ReturnedCount { get; init; }
    public string? DiscountsJson { get; init; }
}
