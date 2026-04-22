namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="SearchOrdersAction"/>.
/// </summary>
public sealed class SearchOrdersOutput
{
    public long TotalCount { get; init; }
    public int ReturnedCount { get; init; }
    public string? OrdersJson { get; init; }
}
