using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace xse_preloader_config.Services;

public class XmlParser
{
    /// <summary>
    /// Parses an XML file to extract plugin preloader configuration data synchronously.
    /// </summary>
    /// <param name="filePath">The file path of the XML document to be parsed.</param>
    /// <returns>
    /// A <see cref="PluginPreloaderData"/> object containing the extracted configuration data.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required XML elements or attributes are missing or invalid within the file.
    /// </exception>
    public PluginPreloaderData Parse(string filePath)
    {
        var document = XDocument.Load(filePath);

        // Parse root element
        var root = document.Element("xSE")
                   ?? throw new InvalidOperationException("Root element 'xSE' not found.");

        // Parse PluginPreloader
        var pluginPreloader = root.Element("PluginPreloader")
                              ?? throw new InvalidOperationException("PluginPreloader section is missing.");

        // Populate PluginPreloaderData
        var preloaderData = new PluginPreloaderData
        {
            OriginalLibrary = pluginPreloader.Element("OriginalLibrary")?.Value ?? string.Empty,
            LoadMethodName = pluginPreloader.Element("LoadMethod")?.Attribute("Name")?.Value ?? "ImportAddressHook",
            ImportLibraryName =
                pluginPreloader.Element("LoadMethod")?.Element("ImportAddressHook")?.Element("LibraryName")?.Value ??
                string.Empty,
            ImportFunctionName =
                pluginPreloader.Element("LoadMethod")?.Element("ImportAddressHook")?.Element("FunctionName")?.Value ??
                string.Empty,
            ThreadNumber =
                int.TryParse(
                    pluginPreloader.Element("LoadMethod")?.Element("OnThreadAttach")?.Element("ThreadNumber")?.Value,
                    out var number)
                    ? number
                    : 0,
            InstallExceptionHandler =
                bool.TryParse(pluginPreloader.Element("InstallExceptionHandler")?.Value, out var install) && install,
            KeepExceptionHandler =
                bool.TryParse(pluginPreloader.Element("KeepExceptionHandler")?.Value, out var keep) && keep,
            LoadDelay = int.TryParse(pluginPreloader.Element("LoadDelay")?.Value, out var loadDelay) ? loadDelay : 0,
            HookDelay = int.TryParse(pluginPreloader.Element("HookDelay")?.Value, out var hookDelay) ? hookDelay : 0,
            Processes = pluginPreloader.Element("Processes")?.Elements("Item")
                .Select(item => new ProcessData
                {
                    Name = item.Attribute("Name")?.Value ?? string.Empty,
                    Allow = bool.TryParse(item.Attribute("Allow")?.Value, out var allow) && allow
                }).Where(p => !string.IsNullOrEmpty(p.Name))
                .ToList()
        };

        return preloaderData;
    }

    /// <summary>
    /// Asynchronously parses an XML file and extracts plugin preloader configuration data.
    /// </summary>
    /// <param name="filePath">The path to the XML file to be parsed.</param>
    /// <returns>
    /// A <see cref="PluginPreloaderData"/> object containing the parsed data from the XML file.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the required XML elements or attributes are missing or invalid in the file.
    /// </exception>
    public async Task<PluginPreloaderData> ParseAsync(string filePath)
    {
        // Load the XML document asynchronously
        using var fileStream = File.OpenRead(filePath);
        var document = await XDocument.LoadAsync(fileStream, LoadOptions.None, CancellationToken.None);

        // Parse root element
        var root = document.Element("xSE")
                   ?? throw new InvalidOperationException("Root element 'xSE' not found.");

        // Parse PluginPreloader
        var pluginPreloader = root.Element("PluginPreloader")
                              ?? throw new InvalidOperationException("PluginPreloader section is missing.");

        // Populate PluginPreloaderData
        var preloaderData = new PluginPreloaderData
        {
            OriginalLibrary = pluginPreloader.Element("OriginalLibrary")?.Value ?? string.Empty,
            LoadMethodName = pluginPreloader.Element("LoadMethod")?.Attribute("Name")?.Value ?? "ImportAddressHook",
            ImportLibraryName =
                pluginPreloader.Element("LoadMethod")?.Element("ImportAddressHook")?.Element("LibraryName")?.Value ??
                string.Empty,
            ImportFunctionName =
                pluginPreloader.Element("LoadMethod")?.Element("ImportAddressHook")?.Element("FunctionName")?.Value ??
                string.Empty,
            ThreadNumber =
                int.TryParse(
                    pluginPreloader.Element("LoadMethod")?.Element("OnThreadAttach")?.Element("ThreadNumber")?.Value,
                    out var number)
                    ? number
                    : 0,
            InstallExceptionHandler =
                bool.TryParse(pluginPreloader.Element("InstallExceptionHandler")?.Value, out var install) && install,
            KeepExceptionHandler =
                bool.TryParse(pluginPreloader.Element("KeepExceptionHandler")?.Value, out var keep) && keep,
            LoadDelay = int.TryParse(pluginPreloader.Element("LoadDelay")?.Value, out var loadDelay) ? loadDelay : 0,
            HookDelay = int.TryParse(pluginPreloader.Element("HookDelay")?.Value, out var hookDelay) ? hookDelay : 0,
            Processes = pluginPreloader.Element("Processes")?.Elements("Item")
                .Select(item => new ProcessData
                {
                    Name = item.Attribute("Name")?.Value ?? string.Empty,
                    Allow = bool.TryParse(item.Attribute("Allow")?.Value, out var allow) && allow
                }).Where(p => !string.IsNullOrEmpty(p.Name))
                .ToList()
        };

        return preloaderData;
    }
}

/// <summary>
/// Represents the configuration data required for loading and managing a plugin preloader.
/// </summary>
public class PluginPreloaderData
{
    public string OriginalLibrary { get; set; }
    public string LoadMethodName { get; set; }
    public string ImportLibraryName { get; set; }
    public string ImportFunctionName { get; set; }
    public int ThreadNumber { get; set; }
    public bool InstallExceptionHandler { get; set; }
    public bool KeepExceptionHandler { get; set; }
    public int LoadDelay { get; set; }
    public int HookDelay { get; set; }
    public List<ProcessData> Processes { get; set; }
}

public class ProcessData
{
    public string Name { get; set; }
    public bool Allow { get; set; }
    public string Tooltip { get; set; }
}