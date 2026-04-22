namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="RemoveOrderLineAction"/>.
/// </summary>
public sealed class RemoveOrderLineOutput
{
    public Guid OrderId { get; init; }
    public Guid OrderLineId { get; init; }
}
