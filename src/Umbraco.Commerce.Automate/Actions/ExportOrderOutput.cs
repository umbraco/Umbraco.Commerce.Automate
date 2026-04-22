namespace Umbraco.Commerce.Automate.Actions;

/// <summary>
/// Output produced by the <see cref="ExportOrderAction"/>.
/// </summary>
public sealed class ExportOrderOutput
{
    public Guid OrderId { get; init; }
    public string? TemplateAlias { get; init; }
    public string? FileName { get; init; }
    public string? MimeType { get; init; }
    public string? Content { get; init; }
}
