using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;

namespace Umbraco.Commerce.Automate.Triggers;

[Trigger("umbracoCommerce.countryRegionChanged", "Country/Region Changed",
    Description = "Fires when an order's billing or shipping country/region changes.",
    Group = "Commerce",
    Icon = "icon-globe",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class CountryRegionChangedTrigger
    : NotificationTriggerBase<StoreTriggerSettings, CountryRegionChangedTriggerOutput, CommerceOrderCountryRegionChangedNotification>
{
    public CountryRegionChangedTrigger(TriggerInfrastructure infrastructure) : base(infrastructure) { }

    public override IEnumerable<TriggerEvent> MapEvent(CommerceOrderCountryRegionChangedNotification notification)
    {
        var order = notification.Order;
        yield return new TriggerEvent<CountryRegionChangedTriggerOutput>
        {
            TriggerAlias = Alias,
            InitiatorType = "system",
            IdempotencyKey = GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode()),
            Output = new CountryRegionChangedTriggerOutput
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                StoreId = order.StoreId,
                Context = notification.Context,
                PreviousCountryId = notification.CountryId.From,
                NewCountryId = notification.CountryId.To,
                PreviousRegionId = notification.RegionId.From,
                NewRegionId = notification.RegionId.To,
            },
        };
    }
}
