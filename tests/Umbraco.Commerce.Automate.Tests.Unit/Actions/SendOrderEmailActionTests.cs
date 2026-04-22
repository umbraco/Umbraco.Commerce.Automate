using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class SendOrderEmailActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IEmailTemplateService> _emailTemplateService = new();

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<SendOrderEmailAction>()
            .WithService(_orderService.Object)
            .WithService(_emailTemplateService.Object)
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
            .WithSettings(new SendOrderEmailSettings { OrderId = Guid.NewGuid().ToString(), EmailTemplateAlias = "" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
