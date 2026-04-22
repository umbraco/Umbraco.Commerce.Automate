using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="ChangeOrderStatusAction"/>.
/// </summary>
public sealed class ChangeOrderStatusSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to update.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new order status ID.
    /// </summary>
    [Field(Label = "Status ID", Description = "The new order status GUID.", SortOrder = 1, SupportsBindings = true)]
    public string OrderStatusId { get; set; } = string.Empty;
}
