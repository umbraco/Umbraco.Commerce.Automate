using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Adds tags to a Commerce order.
/// </summary>
[Action("umbracoCommerce.tagOrder", "Tag Order",
    Description = "Adds tags to a Commerce order.",
    Group = "Commerce",
    Icon = "icon-tags")]
public sealed class TagOrderAction : ActionBase<TagOrderSettings, TagOrderOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;

    public TagOrderAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IUnitOfWorkProvider uowProvider)
        : base(infrastructure)
    {
        _orderService = orderService;
        _uowProvider = uowProvider;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<TagOrderSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (string.IsNullOrWhiteSpace(settings.Tags))
        {
            return ActionResult.Failed(
                new ArgumentException("At least one tag is required."),
                StepRunErrorCategory.Validation);
        }

        var tags = settings.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        await _uowProvider.ExecuteAsync(async uow =>
        {
            var writableOrder = await order.AsWritableAsync(uow);
            await writableOrder.AddTagsAsync(tags);
            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new TagOrderOutput
        {
            OrderId = orderId,
            TagsApplied = tags,
        });
    }
}
