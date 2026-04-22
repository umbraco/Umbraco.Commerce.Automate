using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's DiscountCreatedNotification.
/// </summary>
public sealed class CommerceDiscountCreatedNotification(DiscountReadOnly discount) : INotification
{
    public DiscountReadOnly Discount { get; } = discount;
}
