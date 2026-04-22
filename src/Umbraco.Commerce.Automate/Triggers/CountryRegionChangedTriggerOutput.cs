namespace Umbraco.Commerce.Automate.Triggers;

public sealed class CountryRegionChangedTriggerOutput
{
    public Guid OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public Guid StoreId { get; init; }

    /// <summary>
    /// Gets whether this is a "Payment" or "Shipping" country/region change.
    /// </summary>
    public string? Context { get; init; }

    public Guid? PreviousCountryId { get; init; }
    public Guid? NewCountryId { get; init; }
    public Guid? PreviousRegionId { get; init; }
    public Guid? NewRegionId { get; init; }
}
