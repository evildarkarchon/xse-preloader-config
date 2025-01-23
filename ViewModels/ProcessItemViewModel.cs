using ReactiveUI;

namespace xse_preloader_config.ViewModels;

public class ProcessItemViewModel : ReactiveObject
{
    public required string Name { get; set; }
    public bool Allow { get; set; }
    public string? Tooltip { get; init; }
}