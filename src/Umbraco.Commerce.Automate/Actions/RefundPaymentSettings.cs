using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="RefundPaymentAction"/>.
/// </summary>
public sealed class RefundPaymentSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to refund.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the refund amount. When 0 (default), performs a full refund.
    /// </summary>
    [Field(Label = "Refund Amount", Description = "Amount to refund. Use 0 for a full refund.", SortOrder = 1, SupportsBindings = true)]
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// Gets or sets whether to restock refunded products.
    /// </summary>
    [Field(Label = "Restock Products", Description = "Whether to restock products affected by this refund.", SortOrder = 2)]
    public bool RestockProducts { get; set; }
}
