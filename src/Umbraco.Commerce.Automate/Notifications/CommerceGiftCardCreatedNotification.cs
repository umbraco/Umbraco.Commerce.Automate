using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's GiftCardCreatedNotification.
/// </summary>
public sealed class CommerceGiftCardCreatedNotification(GiftCardReadOnly giftCard) : INotification
{
    public GiftCardReadOnly GiftCard { get; } = giftCard;
}
