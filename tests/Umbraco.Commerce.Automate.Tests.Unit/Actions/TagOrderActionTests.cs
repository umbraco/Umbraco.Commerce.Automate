using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class TagOrderActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<TagOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new TagOrderSettings { OrderId = "not-a-guid", Tags = "vip" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyTags_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<TagOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new TagOrderSettings { OrderId = Guid.NewGuid().ToString(), Tags = "" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
