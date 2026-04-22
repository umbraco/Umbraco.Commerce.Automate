using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="AssignOrderToCustomerAction"/>.
/// </summary>
public sealed class AssignOrderToCustomerSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to assign.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the customer reference.
    /// </summary>
    [Field(Label = "Customer Reference", Description = "The unique customer reference to assign the order to.", SortOrder = 1, SupportsBindings = true)]
    public string CustomerReference { get; set; } = string.Empty;
}
