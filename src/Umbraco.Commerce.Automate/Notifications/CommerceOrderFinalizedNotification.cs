using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderFinalizedNotification.
/// Bridges Commerce's event system to Umbraco's INotification pipeline.
/// </summary>
public sealed class CommerceOrderFinalizedNotification : INotification
{
    public OrderReadOnly Order { get; }

    public CommerceOrderFinalizedNotification(OrderReadOnly order) => Order = order;
}
