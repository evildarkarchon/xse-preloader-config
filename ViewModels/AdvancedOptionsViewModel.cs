using ReactiveUI;

namespace xse_preloader_config.ViewModels;

/// <summary>
/// Represents the view model for managing advanced options in the Preloader Configurator application.
/// </summary>
/// <remarks>
/// This view model provides properties for configuring advanced options such as loading methods,
/// delay settings, exception handler behavior, and related configurations. It is designed
/// to integrate seamlessly with the application's reactive framework for dynamic UI updates.
/// </remarks>
public class AdvancedOptionsViewModel : ReactiveObject
{
    public string OriginalLibrary { get; set; } = string.Empty;
    public string LoadMethod { get; set; } = "ImportAddressHook";
    public string ImportLibraryName { get; set; } = string.Empty;
    public string ImportFunctionName { get; set; } = string.Empty;
    public string ThreadNumber { get; set; } = string.Empty;
    public string LoadDelay { get; set; } = "0";
    public string HookDelay { get; set; } = "0";
    public bool InstallExceptionHandler { get; set; } = true;
    public bool KeepExceptionHandler { get; set; }

/*
    public void LoadFromXml(XElement root)
    {
        var pluginPreloader = root.Element("PluginPreloader");
        if (pluginPreloader == null) return;

        OriginalLibrary = pluginPreloader.Element("OriginalLibrary")?.Value ?? string.Empty;
        LoadMethod = pluginPreloader.Element("LoadMethod")?.Attribute("Name")?.Value ?? "ImportAddressHook";

        var importAddressHook = pluginPreloader.Element("LoadMethod")?.Element("ImportAddressHook");
        if (importAddressHook != null)
        {
            ImportLibraryName = importAddressHook.Element("LibraryName")?.Value ?? string.Empty;
            ImportFunctionName = importAddressHook.Element("FunctionName")?.Value ?? string.Empty;
        }

        var onThreadAttach = pluginPreloader.Element("LoadMethod")?.Element("OnThreadAttach");
        if (onThreadAttach != null)
        {
            ThreadNumber = onThreadAttach.Element("ThreadNumber")?.Value ?? string.Empty;
        }

        LoadDelay = pluginPreloader.Element("LoadDelay")?.Value ?? "0";
        HookDelay = pluginPreloader.Element("HookDelay")?.Value ?? "0";
        InstallExceptionHandler =
            bool.TryParse(pluginPreloader.Element("InstallExceptionHandler")?.Value, out var install) && install;
        KeepExceptionHandler =
            bool.TryParse(pluginPreloader.Element("KeepExceptionHandler")?.Value, out var keep) && keep;
    }
*/
}