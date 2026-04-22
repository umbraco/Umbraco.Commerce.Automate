using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderPaymentMethodChangedNotification.
/// </summary>
public sealed class CommerceOrderPaymentMethodChangedNotification(
    OrderReadOnly order,
    ChangingValue<Guid?> paymentMethodId) : INotification
{
    public OrderReadOnly Order { get; } = order;
    public ChangingValue<Guid?> PaymentMethodId { get; } = paymentMethodId;
}
