using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderAssignedToCustomerNotification.
/// </summary>
public sealed class CommerceOrderAssignedToCustomerNotification(
    OrderReadOnly order,
    ChangingValue<string> customerReference) : INotification
{
    public OrderReadOnly Order { get; } = order;
    public ChangingValue<string> CustomerReference { get; } = customerReference;
}
