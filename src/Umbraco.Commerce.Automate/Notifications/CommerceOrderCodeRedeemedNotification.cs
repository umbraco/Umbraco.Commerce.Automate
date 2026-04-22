using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderDiscountCodeRedeemedNotification
/// and OrderGiftCardRedeemedNotification.
/// </summary>
public sealed class CommerceOrderCodeRedeemedNotification(
    OrderReadOnly order,
    string code,
    string codeType) : INotification
{
    public OrderReadOnly Order { get; } = order;
    public string Code { get; } = code;

    /// <summary>
    /// Gets the type of code redeemed: "Discount" or "GiftCard".
    /// </summary>
    public string CodeType { get; } = codeType;
}
