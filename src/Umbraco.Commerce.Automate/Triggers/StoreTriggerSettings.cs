using Umbraco.Automate.Core.Settings;

namespace Umbraco.Commerce.Automate.Triggers;

/// <summary>
/// Settings shared by Commerce triggers for optional store filtering.
/// </summary>
public sealed class StoreTriggerSettings
{
    /// <summary>
    /// Gets or sets the store ID to filter on. Leave blank to match all stores.
    /// </summary>
    [Field(
        Label = "Store",
        Description = "Only fire for this store. Leave blank to match all stores.",
        EditorUiAlias = "Uc.PropertyEditorUi.StorePicker")]
    public string? StoreId { get; set; }
}
