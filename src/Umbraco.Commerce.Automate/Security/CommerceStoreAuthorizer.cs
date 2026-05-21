using Umbraco.Automate.Core.Security;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;
using Umbraco.Extensions;

namespace Umbraco.Commerce.Automate.Security;

/// <summary>
/// Default <see cref="ICommerceStoreAuthorizer"/> backed by Commerce's <see cref="IStoreService"/>.
/// </summary>
internal sealed class CommerceStoreAuthorizer : ICommerceStoreAuthorizer
{
    private readonly IStoreService _storeService;
    private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

    public CommerceStoreAuthorizer(
        IStoreService storeService,
        IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
        _storeService = storeService;
        _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
    }

    public Task<AutomationAuthorizationResult> AuthorizeStoreAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var user = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
        if (user is null)
        {
            return Task.FromResult(AutomationAuthorizationResult.Fail(
                "No backoffice identity is available — Commerce action invoked outside an Automate run."));
        }

        return AuthorizeStoreAsync(user, storeId, cancellationToken);
    }

    public async Task<AutomationAuthorizationResult> AuthorizeStoreAsync(IUser user, Guid storeId, CancellationToken cancellationToken)
    {
        // Admins and the super user bypass per-store allowlists — same as the Commerce
        // backoffice UI, where an admin can access any store regardless of explicit
        // assignment. Anything stricter would mean an admin can't operate on a freshly-
        // created store until they explicitly add themselves, which surprises operators.
        if (user.IsSuper() || user.IsAdmin())
        {
            return AutomationAuthorizationResult.Success;
        }

        var store = await _storeService.GetStoreAsync(storeId);
        if (store is null)
        {
            return AutomationAuthorizationResult.Fail($"Store '{storeId}' not found.");
        }

        // Delegate to Commerce's own StoreExtensions.IsAllowed(IUser) — it knows the
        // canonical identifier formats (user.Key for AllowedUsers, group.Key for
        // AllowedUserRoles) which differ from the integer Id and alias respectively.
        if (store.IsAllowed(user))
        {
            return AutomationAuthorizationResult.Success;
        }

        return AutomationAuthorizationResult.Fail(
            $"Service account is not allowed to access store '{store.Name}' (id {storeId}).");
    }
}
