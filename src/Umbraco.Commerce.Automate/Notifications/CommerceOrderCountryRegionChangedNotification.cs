using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderPaymentCountryRegionChangedNotification
/// and OrderShippingCountryRegionChangedNotification.
/// </summary>
public sealed class CommerceOrderCountryRegionChangedNotification(
    OrderReadOnly order,
    ChangingValue<Guid?> countryId,
    ChangingValue<Guid?> regionId,
    string context) : INotification
{
    public OrderReadOnly Order { get; } = order;
    public ChangingValue<Guid?> CountryId { get; } = countryId;
    public ChangingValue<Guid?> RegionId { get; } = regionId;

    /// <summary>
    /// Gets whether this is a "Payment" or "Shipping" country/region change.
    /// </summary>
    public string Context { get; } = context;
}
