using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class UpdateStockActionTests
{
    private readonly Mock<IStockService> _stockService = new();
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();

    public UpdateStockActionTests()
    {
        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);
    }

    [Fact]
    public async Task InvalidStoreId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<UpdateStockAction>()
            .WithService(_stockService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new UpdateStockSettings { StoreId = "not-a-guid", ProductReference = "p1", Operation = "Set", Value = 10 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyProductReference_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<UpdateStockAction>()
            .WithService(_stockService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new UpdateStockSettings { StoreId = Guid.NewGuid().ToString(), ProductReference = "", Operation = "Set", Value = 10 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task InvalidOperation_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<UpdateStockAction>()
            .WithService(_stockService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new UpdateStockSettings { StoreId = Guid.NewGuid().ToString(), ProductReference = "p1", Operation = "Multiply", Value = 10 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
