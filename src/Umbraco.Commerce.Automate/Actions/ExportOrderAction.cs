using Umbraco.Automate.Core.Actions;
using Umbraco.Automate.Core.Runs;
using Umbraco.Commerce.Automate.Security;
using Umbraco.Commerce.Core.Services;
using Umbraco.Commerce.Core.Templating;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Renders a Commerce export template for an order and returns the rendered content.
/// </summary>
[Action("umbracoCommerce.exportOrder", "Export Order",
    Description = "Renders a Commerce export template for an order (e.g., invoice, packing slip).",
    Group = "Commerce",
    Icon = "icon-download",
    RequiredSections = [Constants.Sections.Commerce])]
public sealed class ExportOrderAction : ActionBase<ExportOrderSettings, ExportOrderOutput>
{
    private readonly IOrderService _orderService;
    private readonly IExportTemplateService _exportTemplateService;
    private readonly ITemplateEngine _templateEngine;
    private readonly ICommerceStoreAuthorizer _storeAuthorizer;

    public ExportOrderAction(
        ActionInfrastructure infrastructure,
        IOrderService orderService,
        IExportTemplateService exportTemplateService,
        ITemplateEngine templateEngine,
        ICommerceStoreAuthorizer storeAuthorizer)
        : base(infrastructure)
    {
        _orderService = orderService;
        _exportTemplateService = exportTemplateService;
        _templateEngine = templateEngine;
        _storeAuthorizer = storeAuthorizer;
    }

    /// <inheritdoc />
    public override async Task<ActionResult> ExecuteAsync(ActionContext context, CancellationToken cancellationToken)
    {
        var settings = context.GetSettings<ExportOrderSettings>();

        if (!Guid.TryParse(settings.OrderId, out var orderId))
        {
            return ActionResult.Failed(
                new ArgumentException("A valid Order ID is required."),
                StepRunErrorCategory.Validation);
        }

        if (string.IsNullOrWhiteSpace(settings.TemplateAlias))
        {
            return ActionResult.Failed(
                new ArgumentException("A template alias is required."),
                StepRunErrorCategory.Validation);
        }

        var order = await _orderService.GetOrderAsync(orderId);
        if (order is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Order '{orderId}' not found."),
                StepRunErrorCategory.Validation);
        }

        var storeAuth = await _storeAuthorizer.AuthorizeStoreAsync(order.StoreId, cancellationToken);
        if (!storeAuth.Authorized)
        {
            return ActionResult.Failed(
                new UnauthorizedAccessException(storeAuth.FailureReason),
                StepRunErrorCategory.Authentication);
        }

        var template = await _exportTemplateService.GetExportTemplateAsync(order.StoreId, settings.TemplateAlias);
        if (template is null)
        {
            return ActionResult.Failed(
                new InvalidOperationException($"Export template '{settings.TemplateAlias}' not found in store."),
                StepRunErrorCategory.Validation);
        }

        var content = await _templateEngine.RenderTemplateViewAsync(
            template.TemplateView,
            order,
            settings.LanguageIsoCode ?? string.Empty,
            cancellationToken);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{template.Alias}_{order.Id}_{timestamp}.{template.FileExtension}";

        return Success(new ExportOrderOutput
        {
            OrderId = orderId,
            TemplateAlias = template.Alias,
            FileName = fileName,
            MimeType = template.FileMimeType,
            Content = content,
        });
    }
}
