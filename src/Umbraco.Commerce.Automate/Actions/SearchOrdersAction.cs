using System.Text.Json;
using System.Text.Json.Nodes;
using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Extensions;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Searches Commerce orders and returns results as JSON.
/// </summary>
[Action("umbracoCommerce.searchOrders", "Search Orders",
    Description = "Searches Commerce orders and returns results as JSON.",
    Group = "Commerce",
    Icon = "icon-search")]
public sealed class SearchOrdersAction : ActionBase<SearchOrdersSettings, SearchOrdersOutput>
{
    private readonly IOrderService _orderService;

    public SearchOrdersAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService)
        : base(infrastructure)
    {
        _orderService = orderService;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SearchOrdersSettings>();

        if (!Guid.TryParse(settings.StoreId, out var storeId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Store ID is required."),
                StepRunErrorCategory.Validation);
        }

        var pageSize = Math.Clamp(settings.PageSize > 0 ? settings.PageSize : 50, 1, 500);

        var result = await _orderService.SearchOrdersAsync(q =>
        {
            var query = q.FromStore(storeId).And(q.IsFinalized());

            if (!string.IsNullOrWhiteSpace(settings.SearchTerm))
            {
                query = query.And(q.SearchableFieldsMatch(settings.SearchTerm));
            }

            return query;
        }, itemsPerPage: pageSize);

        var ordersArray = new JsonArray();
        foreach (var order in result.Items)
        {
            ordersArray.Add(new JsonObject
            {
                ["id"] = order.Id.ToString(),
                ["orderNumber"] = order.OrderNumber,
                ["storeId"] = order.StoreId.ToString(),
                ["customerEmail"] = order.CustomerInfo?.Email,
                ["customerFirstName"] = order.CustomerInfo?.FirstName,
                ["customerLastName"] = order.CustomerInfo?.LastName,
                ["currencyId"] = order.CurrencyId.ToString(),
                ["paymentStatus"] = order.TransactionInfo?.PaymentStatus?.ToString(),
            });
        }

        return Success(new SearchOrdersOutput
        {
            TotalCount = result.TotalItems,
            ReturnedCount = ordersArray.Count,
            OrdersJson = ordersArray.ToJsonString(),
        });
    }
}
