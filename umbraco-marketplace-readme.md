## Umbraco.Commerce.Automate

Umbraco Commerce triggers and actions for Umbraco Automate - react to store events and run Commerce operations from your automations.

### Features

- **15 Triggers** - React to order lifecycle, payment, stock, code redemption, and email events (e.g. Order Finalized, Payment Updated, Stock Changed, Code Redeemed)
- **18 Actions** - Look up, search, and mutate orders, payments, stock, and gift cards from automation steps (e.g. Change Order Status, Capture Payment, Issue Gift Card, Send Order Email)
- **Bridged Notifications** - Commerce events are re-published as CMS notifications so Automate's trigger infrastructure can observe them
- **Zero Configuration** - Self-registers with Umbraco's composition pipeline; no further wiring required

Example: tag an order when a discount code is redeemed, or notify a channel when stock runs low.

### Requirements

- Umbraco CMS 17.x
- Umbraco Commerce 17.x
- Umbraco.Automate 17.0+
- .NET 10.0
