using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="CapturePaymentAction"/>.
/// </summary>
public sealed class CapturePaymentSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order whose authorized payment should be captured.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;
}
