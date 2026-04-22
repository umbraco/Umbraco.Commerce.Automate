using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class SetOrderPropertyActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<SetOrderPropertyAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new SetOrderPropertySettings { OrderId = "not-a-guid", Alias = "trackingNumber", Value = "X1" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyAlias_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<SetOrderPropertyAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new SetOrderPropertySettings { OrderId = Guid.NewGuid().ToString(), Alias = "", Value = "X1" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
