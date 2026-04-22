namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="TagOrderAction"/>.
/// </summary>
public sealed class TagOrderOutput
{
    public Guid OrderId { get; init; }
    public string[]? TagsApplied { get; init; }
}
