namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="SendOrderEmailAction"/>.
/// </summary>
public sealed class SendOrderEmailOutput
{
    public Guid OrderId { get; init; }
    public bool Sent { get; init; }
}
