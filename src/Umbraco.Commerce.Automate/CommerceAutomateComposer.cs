using Microsoft.Extensions.DependencyInjection;
using Umbraco.Automate.Extensions;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Commerce.Automate.Dispatch;
using Umbraco.Commerce.Automate.Notifications.Handlers;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Events.Notification;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Automate;

/// <summary>
/// Registers Commerce event bridge handlers that republish Commerce events
/// as Umbraco CMS notifications for Automate's trigger pipeline.
/// </summary>
public sealed class CommerceAutomateComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        // Per-store authoriser used by Commerce actions (action-side) and by the
        // store-scoped dispatch authoriser (trigger-side) to enforce membership against
        // Umbraco Commerce's AllowedUsers / AllowedUserRoles lists. Without these, the
        // section gate alone would let a workspace member of Store A receive and operate
        // on Store B's orders.
        builder.Services.AddSingleton<ICommerceStoreAuthorizer, CommerceStoreAuthorizer>();
        builder.AutomateTriggerDispatchAuthorizers()
            .Add<StoreScopedTriggerDispatchAuthorizer>();

        var commerce = builder.WithUmbracoCommerceBuilder();

        commerce.WithNotificationEvent<OrderFinalizedNotification>()
            .RegisterHandler<OrderFinalizedBridgeHandler>();

        commerce.WithNotificationEvent<OrderStatusChangedNotification>()
            .RegisterHandler<OrderStatusChangedBridgeHandler>();

        commerce.WithNotificationEvent<OrderTransactionUpdatedNotification>()
            .RegisterHandler<PaymentUpdatedBridgeHandler>();

        commerce.WithNotificationEvent<StockChangedNotification>()
            .RegisterHandler<StockChangedBridgeHandler>();

        // Order lifecycle
        commerce.WithNotificationEvent<OrderCreatedNotification>()
            .RegisterHandler<OrderCreatedBridgeHandler>();

        commerce.WithNotificationEvent<OrderAssignedToCustomerNotification>()
            .RegisterHandler<OrderAssignedToCustomerBridgeHandler>();

        commerce.WithNotificationEvent<OrderProductAddedNotification>()
            .RegisterHandler<OrderProductAddedBridgeHandler>();

        commerce.WithNotificationEvent<OrderDiscountCodeRedeemedNotification>()
            .RegisterHandler<OrderDiscountCodeRedeemedBridgeHandler>();

        commerce.WithNotificationEvent<OrderGiftCardRedeemedNotification>()
            .RegisterHandler<OrderGiftCardRedeemedBridgeHandler>();

        // Order configuration changes
        commerce.WithNotificationEvent<OrderPaymentMethodChangedNotification>()
            .RegisterHandler<OrderPaymentMethodChangedBridgeHandler>();

        commerce.WithNotificationEvent<OrderShippingMethodChangedNotification>()
            .RegisterHandler<OrderShippingMethodChangedBridgeHandler>();

        commerce.WithNotificationEvent<OrderPaymentCountryRegionChangedNotification>()
            .RegisterHandler<OrderPaymentCountryRegionChangedBridgeHandler>();

        commerce.WithNotificationEvent<OrderShippingCountryRegionChangedNotification>()
            .RegisterHandler<OrderShippingCountryRegionChangedBridgeHandler>();

        // Email events
        commerce.WithNotificationEvent<EmailSentNotification>()
            .RegisterHandler<EmailSentBridgeHandler>();

        commerce.WithNotificationEvent<EmailFailedNotification>()
            .RegisterHandler<EmailFailedBridgeHandler>();

        // Gift card & discount
        commerce.WithNotificationEvent<GiftCardCreatedNotification>()
            .RegisterHandler<GiftCardCreatedBridgeHandler>();

        commerce.WithNotificationEvent<DiscountCreatedNotification>()
            .RegisterHandler<DiscountCreatedBridgeHandler>();
    }
}
