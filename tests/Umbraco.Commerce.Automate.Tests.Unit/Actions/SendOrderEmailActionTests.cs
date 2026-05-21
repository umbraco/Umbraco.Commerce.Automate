using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Core.Security;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class SendOrderEmailActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IEmailTemplateService> _emailTemplateService = new();
    private readonly Mock<ICommerceStoreAuthorizer> _storeAuthorizer = new();

    public SendOrderEmailActionTests()
    {
        _storeAuthorizer
            .Setup(a => a.AuthorizeStoreAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(AutomationAuthorizationResult.Success);
    }

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<SendOrderEmailAction>()
            .WithService(_orderService.Object)
            .WithService(_emailTemplateService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new SendOrderEmailSettings { OrderId = "not-a-guid", EmailTemplateAlias = "confirmation" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyTemplateAlias_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<SendOrderEmailAction>()
            .WithService(_orderService.Object)
            .WithService(_emailTemplateService.Object)
            .WithService(_storeAuthorizer.Object)
            .WithSettings(new SendOrderEmailSettings { OrderId = Guid.NewGuid().ToString(), EmailTemplateAlias = "" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
