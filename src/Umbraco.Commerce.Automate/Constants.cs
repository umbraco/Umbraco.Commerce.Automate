namespace Umbraco.Commerce.Automate;

/// <summary>
/// Constants for the Commerce Automate package.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Umbraco backoffice section aliases referenced by Commerce Automate step types.
    /// </summary>
    public static class Sections
    {
        /// <summary>
        /// The Commerce section — required for any step type that operates on orders,
        /// carts, payments, discounts, gift cards, or product inventory. Store-level
        /// access is additionally enforced at dispatch and action runtime via
        /// <see cref="Security.ICommerceStoreAuthorizer"/>.
        /// </summary>
        public const string Commerce = "commerce";
    }
}
