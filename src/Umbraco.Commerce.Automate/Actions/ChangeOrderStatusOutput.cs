namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="ChangeOrderStatusAction"/>.
/// </summary>
public sealed class ChangeOrderStatusOutput
{
    public Guid OrderId { get; init; }
    public Guid OrderStatusId { get; init; }
}
