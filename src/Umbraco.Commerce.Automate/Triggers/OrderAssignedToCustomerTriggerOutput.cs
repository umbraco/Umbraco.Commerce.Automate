namespace Umbraco.Commerce.Automate.Triggers;

public sealed class OrderAssignedToCustomerTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }
    public string? PreviousCustomerReference { get; init; }
    public string? NewCustomerReference { get; init; }
}
