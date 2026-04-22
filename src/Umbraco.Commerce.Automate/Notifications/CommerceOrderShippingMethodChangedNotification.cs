using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderShippingMethodChangedNotification.
/// </summary>
public sealed class CommerceOrderShippingMethodChangedNotification(
    OrderReadOnly order,
    ChangingValue<Guid?> shippingMethodId) : INotification
{
    public OrderReadOnly Order { get; } = order;
    public ChangingValue<Guid?> ShippingMethodId { get; } = shippingMethodId;
}
