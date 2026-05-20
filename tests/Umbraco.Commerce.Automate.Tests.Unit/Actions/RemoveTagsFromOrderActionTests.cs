using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class RemoveTagsFromOrderActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();

    public RemoveTagsFromOrderActionTests()
    {
        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);
    }

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<RemoveTagsFromOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new RemoveTagsFromOrderSettings { OrderId = "not-a-guid", Tags = "a,b" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyTags_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<RemoveTagsFromOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new RemoveTagsFromOrderSettings { OrderId = Guid.NewGuid().ToString(), Tags = "" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
