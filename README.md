<p align="center">
  <img alt="Umbraco Automate" src="https://raw.githubusercontent.com/umbraco/Umbraco.Commerce.Automate/main/assets/logo-128.png" width="128">
</p>

# Umbraco.Commerce.Automate

Umbraco Commerce triggers and actions for [Umbraco Automate](https://umbraco.com/products/umbraco-automate/).

## Overview

Umbraco.Commerce.Automate is a provider package that connects [Umbraco Commerce](https://umbraco.com/products/umbraco-commerce/) to Umbraco Automate, exposing the Commerce event pipeline and service layer as first-class triggers and actions in Automate flows — for example, tagging an order when a discount code is redeemed, or notifying a channel when stock runs low.

## Key Features

- **15 triggers** — react to order lifecycle, payment, stock, code redemption, and email events
- **18 actions** — look up, search, and mutate orders, payments, stock, and gift cards from automation steps
- **Bridged notifications** — Commerce events are re-published as CMS notifications so Automate's trigger infrastructure can observe them
- **Zero configuration** — `CommerceAutomateComposer` self-registers with Umbraco's composition pipeline

## Installation

```bash
dotnet add package Umbraco.Commerce.Automate
```

No further wiring is required — the composer is auto-discovered by Umbraco's composition system.

## Requirements

- .NET 10.0
- Umbraco CMS 17.x
- Umbraco Commerce 17.x
- Umbraco.Automate 17.0+

## Triggers

Fire an Automate flow when something happens in Commerce.

**Orders**

| Trigger | Fires when… |
|---|---|
| Order Created | A new order is created |
| Order Finalized | An order is finalized |
| Order Status Changed | An order's status changes |
| Order Assigned To Customer | An order is assigned to a customer |
| Product Added To Order | A product is added to an order |

**Order configuration**

| Trigger | Fires when… |
|---|---|
| Payment Method Changed | An order's payment method changes |
| Shipping Method Changed | An order's shipping method changes |
| Country / Region Changed | An order's country or region changes |

**Payments & stock**

| Trigger | Fires when… |
|---|---|
| Payment Updated | A payment status is updated |
| Stock Changed | A product's stock level changes |

**Codes**

| Trigger | Fires when… |
|---|---|
| Code Redeemed | A discount or gift card code is redeemed |
| Gift Card Created | A gift card is created |
| Discount Created | A discount is created |

**Email**

| Trigger | Fires when… |
|---|---|
| Email Sent | A Commerce email is sent |
| Email Failed | A Commerce email fails to send |

## Actions

Run Commerce operations from an Automate flow.

**Order lookup & search**

| Action | What it does |
|---|---|
| Get Order | Fetches an order by ID |
| Search Orders | Searches orders by criteria |
| Search Discounts | Searches discounts |
| Search Gift Cards | Searches gift cards |

**Order mutation**

| Action | What it does |
|---|---|
| Add Product To Order | Adds a product to an order |
| Remove Order Line | Removes a line from an order |
| Assign Order To Customer | Assigns an order to a customer |
| Change Order Status | Changes an order's status |
| Finalize Order | Finalizes an order |
| Set Order Property | Sets a property on an order |
| Tag Order / Remove Tags From Order | Adds or removes order tags |
| Export Order | Exports an order |
| Send Order Email | Sends a Commerce email for an order |

**Payments, inventory & gift cards**

| Action | What it does |
|---|---|
| Capture Payment | Captures an authorized payment |
| Refund Payment | Refunds a payment |
| Update Stock | Updates a product's stock level |
| Issue Gift Card | Issues a new gift card |

## How It Works

Commerce publishes its own event notifications rather than CMS notifications. This package bridges Commerce's event pipeline into Umbraco's `IEventAggregator` so Automate's trigger infrastructure can observe them:

```
Commerce event → BridgeHandler → IEventAggregator notification → Automate trigger
```

## Development

```bash
dotnet restore
dotnet build
dotnet test
```

### Project layout

```
src/
  Umbraco.Commerce.Automate/         # Package source (triggers, actions, bridge handlers)
tests/
  Umbraco.Commerce.Automate.Tests.Unit/
setup-demo.ps1                       # Scaffolds a runnable demo site under demo/ (git-ignored)
```

### Demo site

The demo is not committed to this repo. To scaffold one locally:

```pwsh
./setup-demo.ps1
```

This clones the official [Umbraco Commerce demo store](https://github.com/umbraco/Umbraco.Commerce.DemoStore) into `./demo`, references the local `Umbraco.Commerce.Automate`, and generates `Umbraco.Commerce.Automate.local.slnx` combining src + tests + demo. Run `./setup-demo.ps1 -?` for full options.

## License

MIT — see [LICENSE](LICENSE) for details.
