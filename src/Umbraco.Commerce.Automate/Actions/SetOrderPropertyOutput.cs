namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="SetOrderPropertyAction"/>.
/// </summary>
public sealed class SetOrderPropertyOutput
{
    public Guid OrderId { get; init; }
    public string? Alias { get; init; }
    public string? Value { get; init; }
}
