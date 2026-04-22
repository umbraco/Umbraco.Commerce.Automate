using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Automate.Testing;
using Umbraco.Commerce.Automate.Actions;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Tests.Unit.Actions;

public class RefundPaymentActionTests
{
    private readonly Mock<IOrderService> _orderService = new();
    private readonly Mock<IPaymentService> _paymentService = new();
    private readonly Mock<IUnitOfWorkProvider> _uowProvider = new();

    [Fact]
    public async Task InvalidOrderId_ReturnsValidationError()
    {
        var result = await ActionTestHarness.For<RefundPaymentAction>()
            .WithService(_orderService.Object)
            .WithService(_paymentService.Object)
            .WithService(_uowProvider.Object)
            .WithSettings(new RefundPaymentSettings { OrderId = "not-a-guid" })
            .ExecuteAsync();

        result.Status.ShouldBe(ActionResultStatus.Failed);
        result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
    }
}
