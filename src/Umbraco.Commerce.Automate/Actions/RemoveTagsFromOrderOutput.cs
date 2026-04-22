namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="RemoveTagsFromOrderAction"/>.
/// </summary>
public sealed class RemoveTagsFromOrderOutput
{
    public Guid OrderId { get; init; }
    public string[] TagsRemoved { get; init; } = [];
}
