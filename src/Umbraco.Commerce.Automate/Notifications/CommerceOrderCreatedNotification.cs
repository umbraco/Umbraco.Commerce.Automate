using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderCreatedNotification.
/// </summary>
public sealed class CommerceOrderCreatedNotification(OrderReadOnly order) : INotification
{
    public OrderReadOnly Order { get; } = order;
}
