namespace Umbraco.Commerce.Automate.Triggers;

public sealed class CodeRedeemedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public string? Code { get; init; }
    public string? CodeType { get; init; }
}
