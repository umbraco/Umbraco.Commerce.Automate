using Umbraco.Automate.Core.Settings;
using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Triggers;

namespace Umbraco.Commerce.Automate.Tests.Unit.Triggers;

public class OrderFinalizedTriggerTests
{
    private readonly OrderFinalizedTrigger _trigger = new(
        new TriggerInfrastructure(Mock.Of<IEditableModelResolver>()));

    [Fact]
    public void HasCorrectAlias()
        => _trigger.Alias.ShouldBe("umbracoCommerce.orderFinalized");

    [Fact]
    public void HasCorrectName()
        => _trigger.Name.ShouldBe("Order Finalized");

    [Fact]
    public void HasSettingsType()
        => _trigger.SettingsType.ShouldBe(typeof(StoreTriggerSettings));

    [Fact]
    public void HasOutputType()
        => _trigger.OutputType.ShouldBe(typeof(OrderFinalizedTriggerOutput));
}
