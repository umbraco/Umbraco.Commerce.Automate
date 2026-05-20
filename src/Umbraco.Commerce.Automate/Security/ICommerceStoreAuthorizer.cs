using Umbraco.Automate.Core.Security;
using Umbraco.Cms.Core.Models.Membership;

namespace Umbraco.Commerce.Automate.Security;

/// <summary>
/// Authorises a user — typically the workspace service account — against a Umbraco Commerce
/// store's <c>AllowedUsers</c> / <c>AllowedUserRoles</c> list. Mirrors the shape of
/// <see cref="IAutomationActionAuthorizer"/>: ambient overloads resolve the identity from
/// the backoffice security accessor set by <c>BackOfficeIdentityMiddleware</c> (action-side),
/// the explicit-user overloads are used by dispatch-time authorisation (trigger-side).
/// </summary>
/// <remarks>
/// Admins and the super user bypass the per-store allowlist — same as the backoffice UI,
/// where an admin can access any store regardless of explicit assignment. The section gate
/// (<c>commerce</c>) is checked separately by the standard Automate pipeline.
/// </remarks>
public interface ICommerceStoreAuthorizer
{
    /// <summary>
    /// Authorises the service account currently set on the ambient backoffice accessor for
    /// the given store. Used by Commerce actions inside their <c>ExecuteAsync</c>.
    /// </summary>
    Task<AutomationAuthorizationResult> AuthorizeStoreAsync(
        Guid storeId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Authorises an explicit user for the given store. Used by the dispatch authoriser,
    /// which resolves the workspace service account before any action middleware has set
    /// the ambient accessor.
    /// </summary>
    Task<AutomationAuthorizationResult> AuthorizeStoreAsync(
        IUser user,
        Guid storeId,
        CancellationToken cancellationToken);
}
