using System.Text.Json.Nodes;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Searches Commerce discounts and returns results as JSON.
/// </summary>
[Action("umbracoCommerce.searchDiscounts", "Search Discounts",
    Description = "Lists Commerce discounts for a store, optionally filtered by name/alias or active state.",
    Group = "Commerce",
    Icon = "icon-search",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class SearchDiscountsAction : ActionBase<SearchDiscountsSettings, SearchDiscountsOutput>
{
    private readonly IDiscountService _discountService;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public SearchDiscountsAction(
        ActionInfrastructure infrastructure,
        IDiscountService discountService,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _discountService = discountService;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SearchDiscountsSettings>();

        if (!Guid.TryParse(settings.StoreId, out var storeId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Store ID is required."),
                StepRunErrorCategory.Validation);
        }

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(storeId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        var discounts = settings.OnlyActive
            ? await _discountService.GetActiveDiscountsAsync(storeId)
            : await _discountService.GetDiscountsAsync(storeId);

        var filtered = discounts;
        if (!string.IsNullOrWhiteSpace(settings.SearchTerm))
        {
            var term = settings.SearchTerm;
            filtered = discounts.Where(d =>
                (d.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (d.Alias?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var discountsArray = new JsonArray();
        foreach (var d in filtered)
        {
            discountsArray.Add(new JsonObject
            {
                ["id"] = d.Id.ToString(),
                ["storeId"] = d.StoreId.ToString(),
                ["alias"] = d.Alias,
                ["name"] = d.Name,
                ["isActive"] = d.IsActive,
                ["startDate"] = d.StartDate?.ToString("O"),
                ["expiryDate"] = d.ExpiryDate?.ToString("O"),
            });
        }

        return Success(new SearchDiscountsOutput
        {
            ReturnedCount = discountsArray.Count,
            DiscountsJson = discountsArray.ToJsonString(),
        });
    }
}
