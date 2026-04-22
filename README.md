# Umbraco Commerce Automate

Umbraco Commerce triggers and actions for [Umbraco Automate](https://umbraco.com), exposing the Umbraco Commerce event pipeline and service layer as building blocks for Automate flows.

## What's in the box

### Triggers

Fire an Automate flow when something happens in Commerce.

**Orders**
- Order Created
- Order Finalized
- Order Status Changed
- Order Assigned To Customer
- Product Added To Order

**Order configuration**
- Payment Method Changed
- Shipping Method Changed
- Country / Region Changed

**Payments & stock**
- Payment Updated
- Stock Changed

**Codes**
- Discount or Gift Card Code Redeemed
- Gift Card Created
- Discount Created

**Email**
- Email Sent
- Email Failed

### Actions

Run Commerce operations from an Automate flow.

**Order lookup & search**
- Get Order
- Search Orders
- Search Discounts
- Search Gift Cards

**Order mutation**
- Add Product To Order
- Remove Order Line
- Assign Order To Customer
- Change Order Status
- Finalize Order
- Set Order Property
- Tag Order / Remove Tags From Order
- Export Order
- Send Order Email

**Payments**
- Capture Payment
- Refund Payment

**Inventory & gift cards**
- Update Stock
- Issue Gift Card

## Installation

```bash
dotnet add package Umbraco.Commerce.Automate
```

The `CommerceAutomateComposer` auto-registers with Umbraco's composition pipeline — no further wiring is required. It bridges Commerce's event notifications into CMS notifications so Automate's trigger infrastructure can observe them.

### Requirements

- Umbraco CMS 17
- Umbraco Commerce 17
- Umbraco Automate 0.1+
- .NET 10

## Repository layout

- `src/Umbraco.Commerce.Automate` — the package
- `tests/Umbraco.Commerce.Automate.Tests.Unit` — unit tests for triggers, actions, and bridge handlers
- `demo/Umbraco.Commerce.Automate.DemoSite` — runnable demo site

## License

[MIT](LICENSE)
