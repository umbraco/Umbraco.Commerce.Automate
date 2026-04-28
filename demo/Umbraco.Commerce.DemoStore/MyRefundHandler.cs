using Microsoft.Extensions.Logging;
using Umbraco.Commerce.Common.Events;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.DemoStore.Web
{
    public class MyRefundHandler : NotificationEventHandlerBase<OrderTransactionUpdatingNotification>
    {
        private readonly ILogger<MyRefundHandler> _logger;

        public MyRefundHandler(ILogger<MyRefundHandler> logger)
            => _logger = logger;

        public override void Handle(OrderTransactionUpdatingNotification notification)
        {
            _ = notification;
        }
    }
}
