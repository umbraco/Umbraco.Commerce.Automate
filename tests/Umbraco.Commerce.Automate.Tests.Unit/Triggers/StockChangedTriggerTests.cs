using Umbraco.Automate.Core.Settings;
using Umbraco.Automate.Core.Triggers;
using Umbraco.Commerce.Automate.Notifications;
using Umbraco.Commerce.Automate.Triggers;
using Umbraco.Commerce.Common.Models;

namespace Umbraco.Commerce.Automate.Tests.Unit.Triggers;

public class StockChangedTriggerTests
{
    private readonly StockChangedTrigger _trigger = new(
        new TriggerInfrastructure(Mock.Of<IEditableModelResolver>()));

    [Fact]
    public void HasCorrectAlias()
        => _trigger.Alias.ShouldBe("umbracoCommerce.stockChanged");

    [Fact]
    public void HasCorrectName()
        => _trigger.Name.ShouldBe("Stock Changed");

    [Fact]
    public void HasSettingsType()
        => _trigger.SettingsType.ShouldBe(typeof(StoreTriggerSettings));

    [Fact]
    public void HasOutputType()
        => _trigger.OutputType.ShouldBe(typeof(StockChangedTriggerOutput));

    [Fact]
    public void MapEvent_ProducesEventWithCorrectOutput()
    {
        var storeId = Guid.NewGuid();
        var notification = new CommerceStockChangedNotification(
            storeId, "PROD-001", "VAR-A",
            new ChangingValue<decimal?>(10m, 5m));

        var events = _trigger.MapEvent(notification).ToList();

        events.Count.ShouldBe(1);

        var evt = events[0].ShouldBeOfType<TriggerEvent<StockChangedTriggerOutput>>();
        evt.TriggerAlias.ShouldBe("umbracoCommerce.stockChanged");
        evt.InitiatorType.ShouldBe("system");
        evt.Output.StoreId.ShouldBe(storeId);
        evt.Output.ProductReference.ShouldBe("PROD-001");
        evt.Output.ProductVariantReference.ShouldBe("VAR-A");
        evt.Output.PreviousStock.ShouldBe(10m);
        evt.Output.NewStock.ShouldBe(5m);
    }
}
