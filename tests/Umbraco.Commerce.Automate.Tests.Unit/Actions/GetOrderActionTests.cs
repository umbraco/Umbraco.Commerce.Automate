using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class GetOrderActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();

    public GetOrderActionTests()
    {
        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);
    }

    [Fact]
    public async Task NeitherOrderIdNorOrderNumber_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<GetOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new GetOrderSettings())
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<GetOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new GetOrderSettings { OrderId = "not-a-guid" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task OrderNumberWithoutStoreId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<GetOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new GetOrderSettings { OrderNumber = "1001" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
