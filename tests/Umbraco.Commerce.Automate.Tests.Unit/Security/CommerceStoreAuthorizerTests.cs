using Umbraco.Automate.Core.Security;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;
using CmsConstants = Umbraco.Cms.Core.Constants;

namespace Umbraco.Commerce.Automate.Tests.Unit.Security;

public class CommerceStoreAuthorizerTests
{
    private readonly Mock<IStoreService> _storeService = new();
    private readonly Mock<IBackOfficeSecurityAccessor> _backOfficeSecurityAccessor = new();
    private readonly Mock<IBackOfficeSecurity> _backOfficeSecurity = new();
    private readonly CommerceStoreAuthorizer _sut;

    public CommerceStoreAuthorizerTests()
    {
        _backOfficeSecurityAccessor.SetupGet(a => a.BackOfficeSecurity).Returns(_backOfficeSecurity.Object);
        _sut = new CommerceStoreAuthorizer(_storeService.Object, _backOfficeSecurityAccessor.Object);
    }

    [Fact]
    public async Task AuthorizeStoreAsync_Ambient_NoCurrentUser_ReturnsFail()
    {
        // Ambient overload is invoked from action middleware after BackOfficeIdentityMiddleware
        // has set the service-account identity. If no identity is set (e.g. action called
        // outside an automation run), the authoriser must refuse rather than fall through.
        _backOfficeSecurity.SetupGet(s => s.CurrentUser).Returns((IUser?)null);

        var result = await _sut.AuthorizeStoreAsync(Guid.NewGuid(), CancellationToken.None);

        result.Authorized.ShouldBeFalse();
        result.FailureReason.ShouldContain("No backoffice identity");
        _storeService.Verify(s => s.GetStoreAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AuthorizeStoreAsync_AdminUser_BypassesAllowlist()
    {
        // Admins always pass — same as the Commerce backoffice, where an admin can access
        // any store regardless of explicit AllowedUsers / AllowedUserRoles assignment.
        // IStoreService must not be consulted (skip the round-trip).
        var admin = BuildUser(CmsConstants.Security.AdminGroupAlias);

        var result = await _sut.AuthorizeStoreAsync(admin, Guid.NewGuid(), CancellationToken.None);

        result.Authorized.ShouldBeTrue();
        _storeService.Verify(s => s.GetStoreAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AuthorizeStoreAsync_SuperUser_BypassesAllowlist()
    {
        // The super user also bypasses — same convention as Umbraco's section /
        // start-node checks. As of v18, IUser.IsSuper() is keyed off the user's Key
        // (SuperUserKey) rather than the integer id (-1).
        var superUser = BuildUser(groupAlias: "editor", key: CmsConstants.Security.SuperUserKey);

        var result = await _sut.AuthorizeStoreAsync(superUser, Guid.NewGuid(), CancellationToken.None);

        result.Authorized.ShouldBeTrue();
        _storeService.Verify(s => s.GetStoreAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task AuthorizeStoreAsync_StoreNotFound_ReturnsFailWithStoreId()
    {
        // A non-admin user with a missing store id (deleted? typoed?) must not silently
        // allow — fail closed so the failure surfaces in dispatch / step logs.
        var user = BuildUser("editor");
        var storeId = Guid.NewGuid();
        _storeService.Setup(s => s.GetStoreAsync(storeId)).ReturnsAsync((Umbraco.Commerce.Core.Models.StoreReadOnly?)null);

        var result = await _sut.AuthorizeStoreAsync(user, storeId, CancellationToken.None);

        result.Authorized.ShouldBeFalse();
        result.FailureReason.ShouldContain(storeId.ToString());
    }

    private static IUser BuildUser(string groupAlias, int id = 42, Guid? key = null)
    {
        var group = new Mock<IReadOnlyUserGroup>();
        group.SetupGet(g => g.Alias).Returns(groupAlias);

        var user = new Mock<IUser>();
        user.SetupGet(u => u.Id).Returns(id);
        user.SetupGet(u => u.Key).Returns(key ?? Guid.NewGuid());
        user.SetupGet(u => u.Groups).Returns(new[] { group.Object });
        return user.Object;
    }
}
