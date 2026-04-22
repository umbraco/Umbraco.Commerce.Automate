using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Core.Templating;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class ExportOrderActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IExportTemplateService> _exportTemplateService = new();
    private readonly Mock<ITemplateEngine> _templateEngine = new();

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<ExportOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_exportTemplateService.Object)
            .WithService(_templateEngine.Object)
            .WithSettings(new ExportOrderSettings { OrderId = "not-a-guid", TemplateAlias = "invoice" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }

    [Fact]
    public async Task EmptyTemplateAlias_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<ExportOrderAction>()
            .WithService(_orderService.Object)
            .WithService(_exportTemplateService.Object)
            .WithService(_templateEngine.Object)
            .WithSettings(new ExportOrderSettings { OrderId = Guid.NewGuid().ToString(), TemplateAlias = "" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
