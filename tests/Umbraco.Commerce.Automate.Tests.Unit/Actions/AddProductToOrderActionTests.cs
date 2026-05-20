using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class AddProductToOrderActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();

    public AddProductToOrderActionTests()
    {
        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);
    }

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<AddProductToOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new AddProductToOrderSettings { OrderId = "not-a-guid", ProductReference = "p1", Quantity = 1 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyProductReference_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<AddProductToOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new AddProductToOrderSettings { OrderId = Guid.NewGuid().ToString(), ProductReference = "", Quantity = 1 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task NonPositiveQuantity_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<AddProductToOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new AddProductToOrderSettings { OrderId = Guid.NewGuid().ToString(), ProductReference = "p1", Quantity = 0 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
