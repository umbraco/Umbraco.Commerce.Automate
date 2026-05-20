using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class SearchDiscountsActionTests
{
    private readonly Mock<IDiscountService> _discountService = new();
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();

    public SearchDiscountsActionTests()
    {
        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);
    }

    [Fact]
    public async Task InvalidStoreId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<SearchDiscountsAction>()
            .WithService(_discountService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new SearchDiscountsSettings { StoreId = "not-a-guid" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
