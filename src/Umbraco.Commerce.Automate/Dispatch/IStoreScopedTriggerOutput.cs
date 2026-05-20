namespace Umbraco.Commerce.Automate.Dispatch;

/// <summary>
/// Marker for Commerce trigger outputs whose event subject belongs to a specific store.
/// Surfaces the store key so <see cref="StoreScopedTriggerDispatchAuthorizer"/> can gate
/// dispatch against the workspace service account's <c>AllowedUsers</c> /
/// <c>AllowedUserRoles</c> membership on that store.
/// </summary>
/// <remarks>
/// Internal because the contract is between Commerce.Automate's outputs and its own
/// authoriser. All Commerce events except the email-event triggers (Sent / Failed) carry
/// a store id.
/// </remarks>
internal interface IStoreScopedTriggerOutput
{
    /// <summary>
    /// Returns the store key the event refers to.
    /// </summary>
    Guid GetStoreId();
}
