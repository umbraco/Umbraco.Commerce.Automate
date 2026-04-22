using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Settings for the <see cref="ExportOrderAction"/>.
/// </summary>
public sealed class ExportOrderSettings
{
    /// <summary>
    /// Gets or sets the order ID.
    /// </summary>
    [Field(Label = "Order ID", Description = "The order to export.", SupportsBindings = true)]
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the export template alias.
    /// </summary>
    [Field(Label = "Template Alias", Description = "Alias of the Commerce export template to render.", SortOrder = 1, SupportsBindings = true)]
    public string TemplateAlias { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the language ISO code for translation, e.g. en-US. Optional.
    /// </summary>
    [Field(Label = "Language", Description = "Optional language ISO code (e.g. en-US) for template translation.", SortOrder = 2, SupportsBindings = true)]
    public string? LanguageIsoCode { get; set; }
}
