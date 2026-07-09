# Umbraco.Commerce.Automate

Satellite add-on package for **Umbraco.Automate** (Umbraco's core workflow/automation product). This
repo contributes Commerce-specific *triggers* and *actions* so an Automate flow can react to Commerce
events (order created, payment updated, stock changed, ...) and drive Commerce operations (capture a
payment, tag an order, issue a gift card, ...).

- GitHub: `umbraco/Umbraco.Commerce.Automate`
- Azure DevOps: project "Umbraco Commerce", pipeline definition **724**
- Package: `Umbraco.Commerce.Automate` (NuGet), self-registering — no manual wiring required by consumers
- Requires: .NET 10, Umbraco CMS 18.x, Umbraco Commerce 18.x, Umbraco.Automate 18.0+ (main branch; see
  Teamwork/Workflow for the v17 line)

This is a small, single-product repo: one library project, one test project, no frontend, no database
migrations of its own. Read this file in full before making changes — it is short by design.

## Architecture

### The bridge: Commerce events → CMS notifications → Automate triggers

Umbraco Commerce publishes its own event-notification pipeline (`Umbraco.Commerce.Core.Events.Notification`),
which is a *different* pipeline from the CMS's `Umbraco.Cms.Core.Events` / `IEventAggregator` that
Automate's trigger infrastructure listens on. This package exists almost entirely to bridge the two:

```
Commerce event (e.g. OrderCreatedNotification)
  → *BridgeHandler : NotificationEventHandlerBase<TCommerceEvent>   (src/.../Notifications/Handlers/)
  → wraps it in a CMS INotification (e.g. CommerceOrderCreatedNotification)  (src/.../Notifications/)
  → IEventAggregator.PublishAsync(...)
  → *Trigger : NotificationTriggerBase<TSettings, TOutput, TNotification>   (src/.../Triggers/)
  → MapEvent(...) yields TriggerEvent<TOutput>                              → Automate flow fires
```

Every bridge handler is a thin, one-line translation — see
`src/Umbraco.Commerce.Automate/Notifications/Handlers/OrderCreatedBridgeHandler.cs`:

```csharp
internal sealed class OrderCreatedBridgeHandler(IEventAggregator eventAggregator)
    : NotificationEventHandlerBase<OrderCreatedNotification>
{
    public override async Task HandleAsync(OrderCreatedNotification evt, CancellationToken cancellationToken)
        => await eventAggregator.PublishAsync(new CommerceOrderCreatedNotification(evt.Order), cancellationToken);
}
```

All 15 bridge handlers are registered by hand in
`src/Umbraco.Commerce.Automate/CommerceAutomateComposer.cs` via `commerce.WithNotificationEvent<T>().RegisterHandler<THandler>()`.
**Adding a new trigger means adding a line here** — nothing discovers bridge handlers by convention.

### Composer — the only wiring surface

`CommerceAutomateComposer : IComposer` (src/Umbraco.Commerce.Automate/CommerceAutomateComposer.cs) is
the single composition entry point, auto-discovered by Umbraco's `IComposer` scanning. It registers:

1. `ICommerceStoreAuthorizer` → `CommerceStoreAuthorizer` (singleton) — action-side store gating
2. `StoreScopedTriggerDispatchAuthorizer` into `builder.AutomateTriggerDispatchAuthorizers()` — trigger-side store gating
3. All 15 `WithNotificationEvent<T>().RegisterHandler<T>()` bridge registrations

Triggers and Actions themselves are **not** registered here — `[Trigger]` / `[Action]` attributed classes
are discovered by Automate's own attribute scanning (see below).

### Triggers (`src/Umbraco.Commerce.Automate/Triggers/`)

Each trigger is `[Trigger("umbracoCommerce.<alias>", "<Display Name>", Group = "Commerce", RequiredSections = [Constants.Sections.Commerce])]`
on a `sealed class X : NotificationTriggerBase<TSettings, TOutput, TNotification>`. Most use the shared
`StoreTriggerSettings` (src/Umbraco.Commerce.Automate/Triggers/StoreTriggerSettings.cs) which exposes an
optional `StoreId` filter via a `Uc.PropertyEditorUi.StorePicker` field editor. Email triggers
(`EmailSentTrigger`, `EmailFailedTrigger`) use `object` as their settings type — they have no store
concept to filter on.

`MapEvent(TNotification)` yields zero or more `TriggerEvent<TOutput>`. `OrderCreatedTrigger`
(src/Umbraco.Commerce.Automate/Triggers/OrderCreatedTrigger.cs) is the canonical example, including the
idempotency-key pattern: `GenerateIdempotencyKey(order.Id, order.UpdateDate.GetHashCode())`.
`EmailSentTrigger` shows the "skip when the underlying condition doesn't hold" pattern
(`if (!notification.Success) yield break;` — src/Umbraco.Commerce.Automate/Triggers/EmailSentTrigger.cs:18).

15 triggers ship today: Order Created, Order Finalized, Order Status Changed, Order Assigned To
Customer, Product Added To Order, Payment Method Changed, Shipping Method Changed, Country/Region
Changed, Payment Updated, Stock Changed, Code Redeemed, Gift Card Created, Discount Created, Email
Sent, Email Failed. Full descriptions are in README.md.

### Actions (`src/Umbraco.Commerce.Automate/Actions/`)

Each action is `[Action("umbracoCommerce.<alias>", "<Display Name>", Group = "Commerce", RequiredSections = [Constants.Sections.Commerce])]`
on a `sealed class X : ActionBase<TSettings, TOutput>`, taking `ActionInfrastructure` plus whatever
Commerce SDK services it needs via constructor DI (`IOrderService`, `IPaymentService`,
`IGiftCardService`, `IUnitOfWorkProvider`, always `ICommerceStoreAuthorizer`). 18 actions ship today —
see README.md for the full table. Every action follows the same shape:

1. Parse/validate `context.GetSettings<TSettings>()` — string IDs from step config are parsed with
   `Guid.TryParse`; failures return `ActionResult.Failed(new ArgumentException(...), StepRunErrorCategory.Validation)`.
2. Load the entity (order/store) via the relevant Commerce service; not-found → `StepRunErrorCategory.Validation`.
3. **Authorize the store** — `await _storeAuthorizer.AuthorizeStoreOrFailAsync(storeId, cancellationToken) is { } failure) return failure;`
   (see Security below). This runs *after* the entity load (so the store id is known) but *before* any
   mutation.
4. For read-only actions (`GetOrderAction`, `SearchOrdersAction`, ...): return `Success(new TOutput { ... })`,
   often alongside a `System.Text.Json.Nodes.JsonObject`/`JsonArray` serialized to a `*Json` output field
   for consumers that want the raw entity (see `GetOrderAction.OrderJson`, `SearchOrdersAction.OrdersJson`).
5. For mutating actions (`TagOrderAction`, `CapturePaymentAction`, `IssueGiftCardAction`, ...): mutate
   through the Commerce **unit-of-work pattern** —
   `await _uowProvider.ExecuteAsync(async uow => { var writable = await order.AsWritableAsync(uow); ...; await _orderService.SaveOrderAsync(writable, ct); uow.Complete(); }, cancellationToken);`.
   This is Commerce SDK convention, not something this repo invented — `AsWritableAsync`/`uow.Complete()`
   must always pair, and `SaveOrderAsync`/`SaveGiftCardAsync` happens *inside* the `uow.ExecuteAsync`
   callback, not after it.

### Store-scoped security (dual enforcement point — read this before adding a trigger or action)

Commerce stores have their own `AllowedUsers`/`AllowedUserRoles` allowlist, independent of the CMS
`commerce` backoffice section permission. A workspace member with access to the `commerce` section
could otherwise see/operate on *every* store's orders through Automate, not just the ones they're
scoped to in Commerce's own UI. This repo enforces store membership at **two** separate points:

- **Trigger/dispatch side** — `StoreScopedTriggerDispatchAuthorizer`
  (src/Umbraco.Commerce.Automate/Dispatch/StoreScopedTriggerDispatchAuthorizer.cs), registered as an
  `ITriggerDispatchAuthorizer`. It pattern-matches `context.TypedOutput` against the internal marker
  interface `IStoreScopedTriggerOutput` (src/Umbraco.Commerce.Automate/Dispatch/IStoreScopedTriggerOutput.cs).
  If the output doesn't implement it, dispatch is allowed unconditionally (this is how the two email
  triggers, which carry no store id, pass through). If it does, `GetStoreId()` is checked against the
  *workspace service account* (not the triggering Commerce actor) via `ICommerceStoreAuthorizer`.
- **Action side** — every action calls `ICommerceStoreAuthorizer.AuthorizeStoreOrFailAsync(...)`
  (extension in src/Umbraco.Commerce.Automate/Security/CommerceStoreAuthorizerExtensions.cs) before
  mutating, resolving the identity from the *ambient* backoffice security accessor.

**Gotcha:** every new trigger output that carries a store id **must** implement
`IStoreScopedTriggerOutput` (a one-line `Guid IStoreScopedTriggerOutput.GetStoreId() => StoreId;`,
see `OrderCreatedTriggerOutput.cs`) or it silently bypasses store-level gating — only the coarse
`commerce` section check would apply. There is no analyzer or test that catches a missing marker
across all trigger outputs; `StoreScopedTriggerDispatchAuthorizerTests.cs` only covers the two shapes
that exist today (with marker / without marker for email events).

Admins and the super user bypass the per-store allowlist entirely
(`CommerceStoreAuthorizer.cs:44`) — this mirrors the Commerce backoffice UI's own behavior
(an admin isn't locked out of a freshly created store), and is intentional, not an oversight.

## Commands

Run from the repo root (`D:\DXP\Commerce\Umbraco.Commerce.Automate`). SDK is pinned via `global.json`
(10.0.100, `rollForward: latestFeature`) — no local SDK install decisions needed.

```bash
dotnet restore Umbraco.Commerce.Automate.slnx
dotnet build Umbraco.Commerce.Automate.slnx
dotnet test Umbraco.Commerce.Automate.slnx
```

Run a single test class/method:

```bash
dotnet test Umbraco.Commerce.Automate.slnx --filter "FullyQualifiedName~GetOrderActionTests"
```

Pack (mirrors CI's release artifact step):

```bash
dotnet pack Umbraco.Commerce.Automate.slnx --configuration Release --output ./artifacts
```

Scaffold a runnable demo site (clones the official Commerce demo store, git-ignored, generates a
combined local solution):

```pwsh
./setup-demo.ps1          # -Force to wipe and re-clone, -Branch to pick a demo store branch
```

This produces `demo/` and `Umbraco.Commerce.Automate.local.slnx` (both git-ignored — see `.gitignore`).
Use the `.local.slnx` file, not the tracked `Umbraco.Commerce.Automate.slnx`, when you need to run the
demo site against local source changes.

There is no lint/format script and no `.editorconfig` beyond what ships with the SDK — don't invent one.

## Test Bench

- Framework: xunit + Moq + Shouldly, `Umbraco.Automate.Testing` for harness helpers. `Using` aliases for
  `Xunit`/`Shouldly`/`Moq` are global (tests csproj) — don't add explicit `using` for these.
- **Actions** are tested via `Umbraco.Automate.Testing.ActionTestHarness`:
  ```csharp
  var result = await ActionTestHarness.For<GetOrderAction>()
      .WithService(_orderService.Object)
      .WithService(_storeAuthorizer.Object)
      .WithSettings(new GetOrderSettings { OrderId = "..." })
      .ExecuteAsync();
  result.Status.ShouldBe(ActionResultStatus.Failed);
  result.ErrorCategory.ShouldBe(StepRunErrorCategory.Validation);
  ```
  `WithService` registers each mocked dependency the action's constructor needs — you must supply one
  per constructor parameter after `ActionInfrastructure` (see any file under `tests/.../Actions/`).
- **Dispatch authorizers** are tested directly against the class with hand-built
  `TriggerDispatchAuthorizationContext` (see `Dispatch/StoreScopedTriggerDispatchAuthorizerTests.cs`),
  using `Umbraco.Automate.Testing.Builders.AutomationBuilder` to construct a minimal `Automation`.
- **Bridge handlers** are tested by constructing the handler directly and verifying the
  `IEventAggregator.PublishAsync` call shape (`Notifications/BridgeHandlerTests.cs`) — currently only
  `StockChangedBridgeHandler` has a test; the other 14 handlers are one-liners covered indirectly
  through the trigger/composer wiring rather than individually.
- Every test that documents a *security-relevant* branch (admin bypass, missing identity, denied
  store) carries a comment explaining *why* that branch exists — see
  `tests/.../Security/CommerceStoreAuthorizerTests.cs`. Follow that convention for new authorization
  tests; a bare assertion without the "why" is easy to regress silently later.
- CI runs the test job on a Windows / Linux / macOS matrix (`.devops/test.yml`) — don't rely on
  Windows-only path or culture behavior. `IssueGiftCardAction` deliberately parses dates with
  `DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal` and `CultureInfo.InvariantCulture`
  for exactly this reason (src/Umbraco.Commerce.Automate/Actions/IssueGiftCardAction.cs:71).

## Style Guide

Nothing unusual beyond standard modern C#: file-scoped namespaces, `ImplicitUsings` + `Nullable`
enabled (Directory.Build.props), `sealed` on every concrete trigger/action/handler/authorizer. Small
immutable notification wrapper types use primary constructors
(`CommerceOrderCreatedNotification(OrderReadOnly order) : INotification`); everything with real logic
(actions, triggers, the authorizer) uses a conventional constructor + field assignment, even where a
primary constructor would fit — match whichever style the surrounding file already uses rather than
mixing both in one class.

`internal` is used deliberately for bridge handlers and `IStoreScopedTriggerOutput` — they're
implementation details of this package's own trigger/dispatch contract, not part of the public
extension surface. Keep new bridge handlers and dispatch-only types `internal` unless there's a
concrete reason external code needs them.

## Error Handling

No custom exception types or global error middleware — actions report failure through Automate's own
vocabulary: `ActionResult.Failed(exception, StepRunErrorCategory.<Validation|Authentication>)`.
`Validation` is used for bad/missing input and not-found entities; `Authentication` is reserved for the
store-authorization failure path (`CommerceStoreAuthorizerExtensions.AuthorizeStoreOrFailAsync`). There
is no category for "Commerce service threw" — an unhandled exception from `IOrderService` etc. will
propagate up through Automate's own action-execution wrapper rather than being caught here; this repo
does not add a try/catch around Commerce SDK calls.

## Security

- See "Store-scoped security" under Architecture — this is the security model of the repo and the
  single most important thing to get right when adding triggers or actions.
- `RequiredSections = [Constants.Sections.Commerce]` on every `[Trigger]`/`[Action]` is the coarse gate
  (workspace must have access to the `commerce` backoffice section at all); the store-authorizer checks
  above are the fine-grained gate (which store within Commerce). Both are required — don't treat the
  section check as sufficient on its own.
- `CommerceStoreAuthorizer` fails closed: no ambient backoffice identity → `Fail`, store not found →
  `Fail`. Never change these to fall through to `Success` — the whole point is that a missing/invalid
  identity must never be treated as "unrestricted."

## Teamwork / Workflow

**Branches:** `main` = current CMS major line (v18, `version.json` → `18.0.1`). `support/17.x` =
previous CMS major line (v17, checked out as a persistent worktree at
`.claude/worktrees/support-17.x`, `version.json` → `17.0.1`). Do not edit files under that worktree path
expecting them to affect `main` — it's a separate checkout of the `support/17.x` branch.

**Releases:** cut as `release/YYYY.MM.N` branches, where `N` is a *shared counter across both lines* for
the month (e.g. `release/2026.07.1` for a v18 release and `release/2026.07.2` for a v17 release in the
same month — not independent per-line counters). `azure-pipelines.yml` triggers on push to
`release/*`/`hotfix/*`/`feature/*` and, via `.devops/build-and-pack.yml` + `.devops/test.yml`, runs
**Build → Test only** (no Publish stage). Publishing the packed `.nupkg` to the MyGet feed is a
**manual** step a human does after CI is green. After that's confirmed, the release branch is merged
`--no-ff` into its target, tagged `release-<version>` (e.g. `release-18.0.0`), a GitHub Release is cut
with `gh release create <tag> --target <branch> --generate-notes`, `version.json` is patch-bumped on
the target branch for the next nightly cycle, and the release branch is deleted.

Two Claude Code skills automate exactly this and should be used rather than reinvented:
- `.claude/skills/release-management/SKILL.md` — cuts the release branch, bumps
  `Directory.Packages.props` Automate ranges + `version.json` to the target major's stable floor.
- `.claude/skills/post-release-cleanup/SKILL.md` — merge-back, tag, GitHub Release, next-cycle version
  bump, branch deletion. Run only after the manual MyGet publish is confirmed.

**Package versioning mechanics:** `Directory.Packages.props` pins `Umbraco.Automate` /
`Umbraco.Automate.Core` / `Umbraco.Automate.Testing` to `[18.0.0, 18.999.999)` on `main` (17.x
equivalents on `support/17.x`). NuGet resolves a range to its **floor**, so this range is what pulls the
stable `18.0.0`/`17.0.0` package rather than a prerelease — a floating `18.0.0-*` would instead resolve
to the *highest prerelease*, which is why release-management always writes an explicit range, not a
floating version. `Umbraco.Commerce.Cms.Startup`'s range is managed independently — release-management
does not touch it.

**NuGet sources** (`nuget.config`): nuget.org + Umbraco Nightly + Umbraco Prereleases (both myget.org),
with package source mapping restricting `Umbraco`/`Umbraco.*` to all three Umbraco-aware sources and
everything else to nuget.org only.

**PR target:** `azure-pipelines.yml`'s `pr.branches.include` is `main` and `dev` — treat those as the
two long-lived integration branches for pull requests (`support/17.x` is not listed as a PR target;
changes there flow through the same release-branch mechanism as `main`, not through feature PRs against
it directly, unless the team's actual practice differs — confirm before assuming otherwise if you're
opening a v17 PR).

**No CONTRIBUTING.md** exists in this repo — this CLAUDE.md and the two skills above are the closest
thing to a contribution guide.

## Edge Cases

- **Email triggers have no store.** `EmailEventTriggerOutput` intentionally does *not* implement
  `IStoreScopedTriggerOutput` — `CommerceEmailNotification` only carries a recipient address and
  template alias, no store id. Don't "fix" this by inventing a store id; the dispatch authorizer
  explicitly short-circuits to `Success` for non-store-scoped outputs and there's a test asserting the
  authorizer is never even called in this case.
- **`GetOrderAction` supports two lookup modes** (by `OrderId` guid, or by `StoreId` + `OrderNumber`) —
  when adding similar dual-mode lookups elsewhere, validate that *exactly one* mode's required fields
  are present before touching Commerce services, as `GetOrderAction.ExecuteAsync` does.
  `SearchOrdersAction.PageSize` is clamped to `[1, 500]` with a default of 50
  (`Math.Clamp(settings.PageSize > 0 ? settings.PageSize : 50, 1, 500)`) — any action taking a
  page-size setting from a step field should follow the same defensive clamp, since step config is
  free-text/user-editable and can be zero, negative, or absurdly large.
- **Gift card code generation** (`IssueGiftCardAction`) defers to
  `_giftCardService.GenerateGiftCardCodeAsync(storeId)` when no explicit code is supplied — don't
  generate codes locally; Commerce owns the format/uniqueness guarantee for its own store.
- **`TagOrderAction` splits `Tags` on commas** with `StringSplitOptions.RemoveEmptyEntries | TrimEntries`
  — the settings model exposes a single delimited string field, not a list, because Automate step
  settings editors are field-based; keep this convention for any future multi-value setting rather than
  introducing a different delimiter or a JSON-encoded array.

## Agentic Workflow

- Read README.md first for the up-to-date trigger/action table — it's the product-facing source of
  truth and should be kept in sync whenever a trigger or action is added, renamed, or removed.
- When adding a **trigger**: add the Commerce-side notification wrapper (`Notifications/Commerce*Notification.cs`),
  the bridge handler (`Notifications/Handlers/*BridgeHandler.cs`), register it in
  `CommerceAutomateComposer.Compose`, add the `[Trigger]` class + `TOutput` under `Triggers/`, and if the
  output carries a store id, implement `IStoreScopedTriggerOutput` on it (see Security above — this is
  the step most likely to be forgotten).
- When adding an **action**: create `Settings`/`Output`/`Action` classes under `Actions/` following the
  4-step shape described in Architecture (validate → load → `AuthorizeStoreOrFailAsync` → execute), and
  inject `ICommerceStoreAuthorizer` even if the action feels "read-only" — every action that resolves a
  store id must authorize it.
- Build once after any change (`dotnet build Umbraco.Commerce.Automate.slnx`) — `GenerateDocumentationFile`
  is `true` for `src` (Directory.Build.props), so missing XML doc comments on public members will
  surface as build warnings, not silently pass.
- Don't touch `.claude/worktrees/support-17.x` as part of a `main`-focused task unless the task is
  explicitly about the v17 line — it's a live worktree of a different branch, not a copy/reference
  directory.
- There is no CI you can trigger locally beyond `dotnet build`/`dotnet test`; don't attempt to simulate
  the SBOM/cdxgen step in `.devops/build-and-pack.yml` — it requires network access and DT_API_KEY/DT_BASE_URL
  secrets that aren't available outside Azure DevOps.

## Project-Specific Notes

- **This repo has no domain model of its own.** Every entity (`OrderReadOnly`, `GiftCard`, `StoreReadOnly`,
  ...) and every mutation primitive (`IUnitOfWorkProvider`, `AsWritableAsync`, `uow.Complete()`) comes
  from the `Umbraco.Commerce.Cms.Startup` package (the Commerce SDK). This repo's own types are limited
  to: notification wrappers, bridge handlers, trigger/action classes, and the two security types
  (`ICommerceStoreAuthorizer`, `IStoreScopedTriggerOutput`). When in doubt about "how do I do X with an
  order," look for the equivalent pattern in an existing action rather than inventing a new access path
  into the Commerce SDK.
- **`Umbraco.Automate.Core` vs `Umbraco.Automate`:** the src project references only
  `Umbraco.Automate.Core` (src/Umbraco.Commerce.Automate/Umbraco.Commerce.Automate.csproj:10) — not the
  umbrella `Umbraco.Automate` package. `Directory.Packages.props` still pins a version range for
  `Umbraco.Automate` itself for consistency with the other Automate satellite repos' shared template;
  it isn't actually consumed by any project here. Don't assume adding a `PackageReference` to
  `Umbraco.Automate` is needed for new triggers/actions — `.Core` is sufficient.
  `Umbraco.Automate.Testing` is referenced only by the test project.
  Codebase is clean of `TODO`/`HACK`/`FIXME` markers as of this writing — there is no backlog of known
  shortcuts to be aware of.
- **The demo site is deliberately not part of the repo.** `demo/` and `*.local.slnx` are git-ignored;
  `setup-demo.ps1` clones `umbraco/Umbraco.Commerce.DemoStore` fresh each time and wires a project
  reference to local `src/`. If you need to manually exercise a trigger/action end-to-end, this is the
  only supported path — there's no seeded test database or fixture project checked into this repo
  itself (the demo store repo does ship a `.bacpac` for v17, visible if you inspect the cloned demo,
  but that's the demo store's asset, not this repo's).
- **Bridge handlers are intentionally dumb.** They do no filtering, no business logic, and no error
  handling beyond what `NotificationEventHandlerBase<T>` provides — all of that lives in the trigger's
  `MapEvent`. If you're tempted to add conditional logic inside a bridge handler (e.g. "only bridge this
  event if X"), it almost certainly belongs in `MapEvent` instead, where `EmailSentTrigger`'s
  `if (!notification.Success) yield break;` already sets the precedent.
- **`Constants.Sections.Commerce`** (src/Umbraco.Commerce.Automate/Constants.cs) is the only constant in
  the package; it exists purely so every `[Trigger]`/`[Action]`'s `RequiredSections` references the same
  string literal rather than each duplicating `"commerce"`.
