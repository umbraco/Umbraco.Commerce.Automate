using Umbraco.Automate.Core.Automations;
using Umbraco.Automate.Core.Dispatch.Authorization;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Core.Triggers;
using Umbraco.Automate.Testing.Builders;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Commerce.Automate.Dispatch;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Automate.Triggers;

namespace Umbraco.Commerce.Automate.Tests.Unit.Dispatch;

public class StoreScopedTriggerDispatchAuthorizerTests
{
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();
    private readonly StoreScopedTriggerDispatchAuthorizer _sut;

    public StoreScopedTriggerDispatchAuthorizerTests()
    {
        _sut = new StoreScopedTriggerDispatchAuthorizer(_storeAuthorizer.Object);
    }

    [Fact]
    public async Task AuthorizeAsync_OutputWithoutMarker_ReturnsSuccess()
    {
        // Email events (EmailSent / EmailFailed) carry no store id and must not be gated by
        // the store-membership check. The authoriser must short-circuit to Success without
        // calling ICommerceStoreAuthorizer.
        var output = new EmailEventTriggerOutput
        {
            ToEmailAddress = "editor@example.com",
            EmailTemplateAlias = "order-confirmation",
            Success = true,
        };

        var result = await _sut.AuthorizeAsync(BuildContext(output), CancellationToken.None);

        result.Authorized.ShouldBeTrue();
        _storeAuthorizer.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AuthorizeAsync_StoreScopedOutput_DelegatesToStoreAuthorizer()
    {
        // Happy path: an OrderCreated output identifies a store; the authoriser routes
        // through ICommerceStoreAuthorizer using the resolved service account.
        var output = new OrderCreatedTriggerOutput
        {
            OrderId = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
        };
        var user = Mock.Of<IUser>();

        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(user, output.StoreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);

        var result = await _sut.AuthorizeAsync(BuildContext(output, user), CancellationToken.None);

        result.Authorized.ShouldBeTrue();
        _storeAuthorizer.Verify(a => a.AuthorizeStoreAsync(user, output.StoreId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AuthorizeAsync_StoreScopedOutputDenied_PropagatesFailureReason()
    {
        // Service account not in the store's AllowedUsers / AllowedUserRoles. The authoriser
        // must propagate the failure reason so the dispatcher log line names the actual block.
        var output = new OrderCreatedTriggerOutput
        {
            OrderId = Guid.NewGuid(),
            StoreId = Guid.NewGuid(),
            CurrencyId = Guid.NewGuid(),
        };

        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<IUser>(), output.StoreId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Fail("Service account is not allowed to access store 'Demo Store'."));

        var result = await _sut.AuthorizeAsync(BuildContext(output), CancellationToken.None);

        result.Authorized.ShouldBeFalse();
        result.FailureReason.ShouldBe("Service account is not allowed to access store 'Demo Store'.");
    }

    private static TriggerDispatchAuthorizationContext BuildContext(object? typedOutput, IUser? user = null)
        => new()
        {
            Trigger = Mock.Of<ITrigger>(),
            TypedOutput = typedOutput,
            ServiceAccount = user ?? Mock.Of<IUser>(),
            Automation = new AutomationBuilder().WithTrigger("any").Build(),
        };
}
