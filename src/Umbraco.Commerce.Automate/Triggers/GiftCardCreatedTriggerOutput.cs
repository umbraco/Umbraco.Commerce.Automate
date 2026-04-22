namespace Umbraco.Commerce.Automate.Triggers;

public sealed class GiftCardCreatedTriggerOutput
{
    public Guid GiftCardId { get; init; }
    public Guid StoreId { get; init; }
    public string? Code { get; init; }
}
