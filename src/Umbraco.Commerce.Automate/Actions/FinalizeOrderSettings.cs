using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="FinalizeOrderAction"/>.
/// </summary>
public sealed class FinalizeOrderSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to finalize.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount authorized by the payment gateway.
    /// </summary>
    [Field(Label = "Amount Authorized", Description = "The amount authorized by the payment gateway.", SortOrder = 1, SupportsBindings = true)]
    public decimal AmountAuthorized { get; set; }

    /// <summary>
    /// Gets or sets the transaction ID from the payment gateway.
    /// </summary>
    [Field(Label = "Transaction ID", Description = "The transaction ID from the payment gateway.", SortOrder = 2, SupportsBindings = true)]
    public string? TransactionId { get; set; }

    /// <summary>
    /// Gets or sets the payment status. One of: Initialized, Authorized, Captured, Cancelled, Refunded, PendingExternalSystem, PartiallyRefunded, Error.
    /// </summary>
    [Field(Label = "Payment Status", Description = "The payment status: Initialized, Authorized, Captured, Cancelled, Refunded, PendingExternalSystem, PartiallyRefunded, or Error.", SortOrder = 3, SupportsBindings = true)]
    public string PaymentStatus { get; set; } = "Captured";

    /// <summary>
    /// Gets or sets the transaction fee charged by the payment gateway. Defaults to 0.
    /// </summary>
    [Field(Label = "Transaction Fee", Description = "The transaction fee charged by the payment gateway.", SortOrder = 4, SupportsBindings = true)]
    public decimal TransactionFee { get; set; }
}
