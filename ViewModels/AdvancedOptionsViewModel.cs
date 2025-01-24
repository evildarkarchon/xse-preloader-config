using System.Xml.Linq;
using xse_preloader_config.Services;

namespace xse_preloader_config.ViewModels;

/// <summary>
/// Represents the view model for managing advanced options in the Preloader Configurator application.
/// </summary>
/// <remarks>
/// This view model provides properties for configuring advanced options such as loading methods,
/// delay settings, exception handler behavior, and related configurations. It is designed
/// to integrate seamlessly with the application's reactive framework for dynamic UI updates.
/// </remarks>
public class AdvancedOptionsViewModel : ViewModelBase
{
    public string OriginalLibrary { get; set; }
    public string LoadMethod { get; set; }
    public string ImportLibraryName { get; set; }
    public string ImportFunctionName { get; set; }
    public int ThreadNumber { get; set; }
    public bool InstallExceptionHandler { get; set; }
    public bool KeepExceptionHandler { get; set; }
    public int LoadDelay { get; set; }
    public int HookDelay { get; set; }

    /// <summary>
    /// Loads the configuration data from the provided PluginPreloaderData instance into the view model's properties.
    /// </summary>
    /// <param name="data">The data object containing configuration information, such as library details, loading method, exception handling settings, and delay configurations.</param>
    public void LoadFromData(PluginPreloaderData data)
    {
        OriginalLibrary = data.OriginalLibrary;
        LoadMethod = data.LoadMethodName;
        ImportLibraryName = data.ImportLibraryName;
        ImportFunctionName = data.ImportFunctionName;
        ThreadNumber = data.ThreadNumber;
        InstallExceptionHandler = data.InstallExceptionHandler;
        KeepExceptionHandler = data.KeepExceptionHandler;
        LoadDelay = data.LoadDelay;
        HookDelay = data.HookDelay;
    }

    /// <summary>
    /// Saves the advanced options configuration to an XML representation.
    /// </summary>
    /// <returns>
    /// An XElement representing the advanced options, including settings for library loading,
    /// method configuration, exception handling preferences, and delay configurations.
    /// </returns>
    public XElement SaveToXml()
    {
        return new XElement("PluginPreloader",
            new XElement("OriginalLibrary", OriginalLibrary),
            new XElement("LoadMethod",
                new XAttribute("Name", LoadMethod),
                new XElement("ImportAddressHook",
                    new XElement("LibraryName", ImportLibraryName),
                    new XElement("FunctionName", ImportFunctionName)
                ),
                new XElement("OnThreadAttach",
                    new XElement("ThreadNumber", ThreadNumber)
                ),
                new XElement("OnProcessAttach")
            ),
            new XElement("InstallExceptionHandler", InstallExceptionHandler),
            new XElement("KeepExceptionHandler", KeepExceptionHandler),
            new XElement("LoadDelay", LoadDelay),
            new XElement("HookDelay", HookDelay)
        );
    }
}