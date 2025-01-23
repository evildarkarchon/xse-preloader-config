using ReactiveUI;

namespace xse_preloader_config.ViewModels;

/// <summary>
/// Represents the view model for a single process item.
/// </summary>
/// <remarks>
/// This class is used to define individual process items with attributes such as a name,
/// a flag indicating if the process is allowed, and an optional tooltip for display purposes.
/// </remarks>
public class ProcessItemViewModel : ReactiveObject
{
    public string? Name { get; set; }
    public bool Allow { get; set; }
    public string? Tooltip { get; init; }
}