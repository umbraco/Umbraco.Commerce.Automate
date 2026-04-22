using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Core.Services;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Sends an email using a Commerce email template for an order.
/// </summary>
[Action("umbracoCommerce.sendOrderEmail", "Send Order Email",
    Description = "Sends an email using a Commerce email template.",
    Group = "Commerce",
    Icon = "icon-message")]
public sealed class SendOrderEmailAction : ActionBase<SendOrderEmailSettings, SendOrderEmailOutput>
{
    private readonly IOrderService _orderService;
    private readonly IEmailTemplateService _emailTemplateService;

    public SendOrderEmailAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IEmailTemplateService emailTemplateService)
        : base(infrastructure)
    {
        _orderService = orderService;
        _emailTemplateService = emailTemplateService;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<SendOrderEmailSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (string.IsNullOrWhiteSpace(settings.EmailTemplateAlias))
        {
            return ActionResult.Failed(
                new ArgumentException("An email template alias is required."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        var template = await _emailTemplateService.GetEmailTemplateAsync(order.StoreId, settings.EmailTemplateAlias);
        if (template is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Email template '{settings.EmailTemplateAlias}' not found in store."),
                StepRunErrorCategory.Validation);
        }

        var sent = await _emailTemplateService.SendEmailAsync(template, order, cancellationToken);

        return Success(new SendOrderEmailOutput
        {
            OrderId = orderId,
            Sent = sent,
        });
    }
}
