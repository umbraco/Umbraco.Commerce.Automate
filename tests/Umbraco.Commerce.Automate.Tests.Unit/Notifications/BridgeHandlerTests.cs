using Umbraco.Cms.Core.Events;
using Umbraco.Commerce.Automate.Notifications;
using Umbraco.Commerce.Automate.Notifications.Handlers;
using Umbraco.Commerce.Common.Models;
using Umbraco.Commerce.Core.Events.Notification;

namespace Umbraco.Commerce.Automate.Tests.Unit.Notifications;

public class BridgeHandlerTests
{
    private readonly Mock<IEventAggregator> _eventAggregator = new();

    [Fact]
    public async Task StockChangedBridgeHandler_PublishesCmsNotification()
    {
        var handler = new StockChangedBridgeHandler(_eventAggregator.Object);
        var storeId = Guid.NewGuid();
        var commerceEvent = new StockChangedNotification(storeId, "PROD-001", "VAR-A", new ChangingValue<decimal?>(10m, 5m));

        await handler.HandleAsync(commerceEvent, CancellationToken.None);

        _eventAggregator.Verify(
            ea => ea.PublishAsync(
                It.Is<CommerceStockChangedNotification>(n =>
                    n.StoreId == storeId &&
                    n.ProductReference == "PROD-001" &&
                    n.ProductVariantReference == "VAR-A" &&
                    n.Value.From == 10m &&
                    n.Value.To == 5m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
