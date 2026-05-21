using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Common;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Sets a custom property value on a Commerce order.
/// </summary>
[Action("umbracoCommerce.setOrderProperty", "Set Order Property",
    Description = "Sets a custom property value on a Commerce order.",
    Group = "Commerce",
    Icon = "icon-edit",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class SetOrderPropertyAction : ActionBase<SetOrderPropertySettings, SetOrderPropertyOutput>
{
    private readonly IOrderService _orderService;
    private readonly IUnitOfWorkProvider _uowProvider;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public SetOrderPropertyAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IUnitOfWorkProvider uowProvider,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _orderService = orderService;
        _uowProvider = uowProvider;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SetOrderPropertySettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (string.IsNullOrWhiteSpace(settings.Alias))
        {
            return ActionResult.Failed(
                new ArgumentException("A property alias is required."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        if (await _storeAuthorizer.AuthorizeStoreOrFailAsync(order.StoreId, cancellationToken) is { } storeAuthFailure)
        {
            return storeAuthFailure;
        }

        await _uowProvider.ExecuteAsync(async uow =>
        {
            var writableOrder = await order.AsWritableAsync(uow);
            await writableOrder.SetPropertyAsync(settings.Alias, settings.Value ?? string.Empty);
            await _orderService.SaveOrderAsync(writableOrder, cancellationToken);
            uow.Complete();
        }, cancellationToken);

        return Success(new SetOrderPropertyOutput
        {
            OrderId = orderId,
            Alias = settings.Alias,
            Value = settings.Value,
        });
    }
}
