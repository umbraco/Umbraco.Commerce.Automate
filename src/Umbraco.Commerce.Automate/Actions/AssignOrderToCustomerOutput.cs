namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="AssignOrderToCustomerAction"/>.
/// </summary>
public sealed class AssignOrderToCustomerOutput
{
    public Guid OrderId { get; init; }
    public string? CustomerReference { get; init; }
}
