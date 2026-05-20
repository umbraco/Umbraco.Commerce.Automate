using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;

namespace Umbraco.Commerce.Automate.Security;

/// <summary>
/// Convenience helpers for the common "check then fail-the-step" store-membership
/// authorisation prelude used by Commerce actions.
/// </summary>
public static class CommerceStoreAuthorizerExtensions
{
    /// <summary>
    /// Authorises the service account against <paramref name="storeId"/> and returns a
    /// failed <see cref="ActionResult"/> when access is denied. Returns <c>null</c> on
    /// success so the caller can short-circuit with
    /// <c>if (await ... is { } failure) return failure;</c>.
    /// </summary>
    public static async Task<ActionResult?> AuthorizeStoreOrFailAsync(
        this ICommerceStoreAuthorizer authorizer,
        Guid storeId,
        CancellationToken cancellationToken)
    {
        var result = await authorizer.AuthorizeStoreAsync(storeId, cancellationToken);
        return result.Authorized
            ? null
            : ActionResult.Failed(
                new UnauthorizedAccessException(result.FailureReason),
                StepRunErrorCategory.Authentication);
    }
}
