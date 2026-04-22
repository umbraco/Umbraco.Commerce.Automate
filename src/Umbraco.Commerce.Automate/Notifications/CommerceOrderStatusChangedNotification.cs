using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderStatusChangedNotification.
/// </summary>
public sealed class CommerceOrderStatusChangedNotification : INotification
{
    public OrderReadOnly Order { get; }
    public ChangingValue<Guid?> OrderStatusId { get; }
    public ChangingValue<OrderStatusCode> OrderStatusCode { get; }

    public CommerceOrderStatusChangedNotification(
        OrderReadOnly order,
        ChangingValue<Guid?> orderStatusId,
        ChangingValue<OrderStatusCode> orderStatusCode)
    {
        Order = order;
        OrderStatusId = orderStatusId;
        OrderStatusCode = orderStatusCode;
    }
}
