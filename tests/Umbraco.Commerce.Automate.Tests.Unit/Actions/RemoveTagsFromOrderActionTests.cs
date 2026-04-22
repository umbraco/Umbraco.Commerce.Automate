using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class RemoveTagsFromOrderActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<RemoveTagsFromOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_uowProvider.Object)
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
            .WithSettings(new RemoveTagsFromOrderSettings { OrderId = Guid.NewGuid().ToString(), Tags = "" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
