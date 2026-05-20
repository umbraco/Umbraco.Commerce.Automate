using System.Text.Json.Nodes;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Searches Commerce gift cards and returns results as JSON.
/// </summary>
[Action("umbracoCommerce.searchGiftCards", "Search Gift Cards",
    Description = "Searches Commerce gift cards and returns results as JSON.",
    Group = "Commerce",
    Icon = "icon-search",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class SearchGiftCardsAction : ActionBase<SearchGiftCardsSettings, SearchGiftCardsOutput>
{
    private readonly IGiftCardService _giftCardService;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public SearchGiftCardsAction(
        ActionInfrastructure infrastructure,
        IGiftCardService giftCardService,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _giftCardService = giftCardService;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SearchGiftCardsSettings>();

        if (!Guid.TryParse(settings.StoreId, out var storeId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Store ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (await _storeAuthorizer.AuthorizeStoreOrFailAsync(storeId, cancellationToken) is { } storeAuthFailure)
        {
            return storeAuthFailure;
        }

        var pageSize = Math.Clamp(settings.PageSize > 0 ? settings.PageSize : 50, 1, 500);

        var result = await _giftCardService.SearchGiftCardsAsync(q =>
        {
            var query = q.FromStore(storeId);

            if (!string.IsNullOrWhiteSpace(settings.SearchTerm))
            {
                query = query.And(q.SearchableFieldsMatch(settings.SearchTerm));
            }

            return query;
        }, itemsPerPage: pageSize);

        var giftCardsArray = new JsonArray();
        foreach (var gc in result.Items)
        {
            var remaining = await gc.RemainingAmount;
            var status = await gc.Status;

            giftCardsArray.Add(new JsonObject
            {
                ["id"] = gc.Id.ToString(),
                ["code"] = gc.Code,
                ["storeId"] = gc.StoreId.ToString(),
                ["currencyId"] = gc.CurrencyId.ToString(),
                ["originalAmount"] = gc.OriginalAmount.Value,
                ["remainingAmount"] = remaining.Value,
                ["expiryDate"] = gc.ExpiryDate?.ToString("O"),
                ["isActive"] = gc.IsActive,
                ["status"] = status.ToString(),
                ["orderId"] = gc.OrderId?.ToString(),
            });
        }

        return Success(new SearchGiftCardsOutput
        {
            TotalCount = result.TotalItems,
            ReturnedCount = giftCardsArray.Count,
            GiftCardsJson = giftCardsArray.ToJsonString(),
        });
    }
}
