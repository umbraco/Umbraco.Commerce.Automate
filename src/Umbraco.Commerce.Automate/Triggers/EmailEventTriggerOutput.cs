namespace Umbraco.Commerce.Automate.Triggers;

public sealed class EmailEventTriggerOutput
{
    public string? ToEmailAddress { get; init; }
    public string? EmailTemplateAlias { get; init; }
    public bool Success { get; init; }
}
