using Umbraco.Automate.Core.Dispatch.Authorization;
using Umbraco.Automate.Core.Security;
using Umbraco.Commerce.Automate.Security;

namespace Umbraco.Commerce.Automate.Dispatch;

/// <summary>
/// Dispatch-time authoriser for Commerce triggers whose output identifies a store. Gates
/// the run against the workspace service account's store membership via
/// <see cref="ICommerceStoreAuthorizer"/>. Without this, a workspace member of Store A
/// could subscribe to and receive verbatim order payloads from Store B.
/// </summary>
internal sealed class StoreScopedTriggerDispatchAuthorizer : ITriggerDispatchAuthorizer
{
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public StoreScopedTriggerDispatchAuthorizer(ICommerceStoreAuthorizer storeAuthorizer)
    {
        _storeAuthorizer = storeAuthorizer;
    }

    public async Task<AutomationAuthorizationResult> AuthorizeAsync(
        TriggerDispatchAuthorizationContext context,
        CancellationToken cancellationToken)
    {
        // Email triggers (EmailSent, EmailFailed) carry no store key — they're section-only.
        // Anything else passing through this authoriser must implement the marker.
        if (context.TypedOutput is not IStoreScopedTriggerOutput storeOutput)
        {
            return AutomationAuthorizationResult.Success;
        }

        return await _storeAuthorizer.AuthorizeStoreAsync(
            context.ServiceAccount, storeOutput.GetStoreId(), cancellationToken);
    }
}
