using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's OrderProductAddedNotification.
/// </summary>
public sealed class CommerceOrderProductAddedNotification(
    OrderReadOnly order,
    string productReference,
    string? productVariantReference,
    decimal quantity) : INotification
{
    public OrderReadOnly Order { get; } = order;
    public string ProductReference { get; } = productReference;
    public string? ProductVariantReference { get; } = productVariantReference;
    public decimal Quantity { get; } = quantity;
}
