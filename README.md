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
- `setup-demo.ps1` — scaffolds a runnable demo site under `demo/` (git-ignored)

## Running the demo

The demo is not committed to this repo. To scaffold one locally:

```pwsh
./setup-demo.ps1
```

This clones the official [Umbraco Commerce demo store](https://github.com/umbraco/Umbraco.Commerce.DemoStore) into `./demo`, adds a project reference from the demo's web project to the local `Umbraco.Commerce.Automate`, and generates `Umbraco.Commerce.Automate.local.slnx` at the repo root combining src + tests + the demo. Both `demo/` and `*.local.slnx` are git-ignored.

Useful parameters:

- `-Branch <ref>` — branch, tag, or commit of the demo store to check out (default `main`)
- `-Path <dir>` — target folder (default `demo`)
- `-Repo <url>` — alternate demo store repo
- `-DemoSubPath <path>` — path within the cloned repo that contains the demo solution (defaults to the repo root)
- `-WebProject <csproj>` — explicit web project to reference Automate from (auto-detected by default)
- `-Force` — wipe and re-clone an existing demo folder

Run `./setup-demo.ps1 -?` for full help.

## License

[MIT](LICENSE)
