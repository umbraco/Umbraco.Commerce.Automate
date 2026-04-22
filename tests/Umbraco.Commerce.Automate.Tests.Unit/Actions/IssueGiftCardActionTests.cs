using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class IssueGiftCardActionTests
{
    private readonly Mock<IGiftCardService> _giftCardService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();

    [Fact]
    public async Task InvalidStoreId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<IssueGiftCardAction>()
            .WithService(_giftCardService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new IssueGiftCardSettings { StoreId = "not-a-guid", CurrencyId = Guid.NewGuid().ToString(), Amount = 10 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task InvalidCurrencyId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<IssueGiftCardAction>()
            .WithService(_giftCardService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new IssueGiftCardSettings { StoreId = Guid.NewGuid().ToString(), CurrencyId = "not-a-guid", Amount = 10 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task NonPositiveAmount_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<IssueGiftCardAction>()
            .WithService(_giftCardService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new IssueGiftCardSettings { StoreId = Guid.NewGuid().ToString(), CurrencyId = Guid.NewGuid().ToString(), Amount = 0 })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task InvalidExpiryDate_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<IssueGiftCardAction>()
            .WithService(_giftCardService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new IssueGiftCardSettings
            {
                StoreId = Guid.NewGuid().ToString(),
                CurrencyId = Guid.NewGuid().ToString(),
                Amount = 10,
                ExpiryDate = "not-a-date",
            })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
