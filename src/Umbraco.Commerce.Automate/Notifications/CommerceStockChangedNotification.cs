using Umbraco.Cms.Core.Notifications;
using Umbraco.Commerce.Common.Models;

namespace Umbraco.Commerce.Automate.Notifications;

/// <summary>
/// CMS notification wrapper for Commerce's StockChangedNotification.
/// </summary>
public sealed class CommerceStockChangedNotification : INotification
{
    public Guid StoreId { get; }
    public string ProductReference { get; }
    public string? ProductVariantReference { get; }
    public ChangingValue<decimal?> Value { get; }

    public CommerceStockChangedNotification(
        Guid storeId,
        string productReference,
        string? productVariantReference,
        ChangingValue<decimal?> value)
    {
        StoreId = storeId;
        ProductReference = productReference;
        ProductVariantReference = productVariantReference;
        Value = value;
    }
}
