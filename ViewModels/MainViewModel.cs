using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using ReactiveUI;

namespace xse_preloader_config.ViewModels;

/// <summary>
/// Represents the main view model for the Preloader Configurator application.
/// </summary>
/// <remarks>
/// This view model serves as the primary hub for data binding between the UI and the underlying logic of the application.
/// It manages advanced options, processes, and commands for opening, saving, and managing XML files.
/// Additionally, it facilitates error handling and visibility toggles for advanced features.
/// </remarks>
public class MainViewModel : ReactiveObject
{
    private readonly Window? _parentWindow;
    private bool _showAdvancedOptions;

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private string _errorMessage = null!;
    public AdvancedOptionsViewModel AdvancedOptions { get; }
    public ProcessesViewModel Processes { get; }

    public bool ShowAdvancedOptions
    {
        get => _showAdvancedOptions;
        set => this.RaiseAndSetIfChanged(ref _showAdvancedOptions, value);
    }

    private string? _currentFilePath;

    private string? CurrentFilePath
    {
        get => _currentFilePath;
        set => this.RaiseAndSetIfChanged(ref _currentFilePath, value);
    }

    public ReactiveCommand<Unit, Unit> OpenXmlCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveXmlCommand { get; }
    public ReactiveCommand<Unit, Task> SaveAsXmlCommand { get; }

    /// <summary>
    /// Represents the main view model for the application's main window interface.
    /// </summary>
    /// <remarks>
    /// The MainViewModel class serves as a view model for managing the core functionality of the application,
    /// including options and processes related to XML configuration workflows.
    /// It provides commands for opening, saving, and managing XML files, as well as handling advanced options,
    /// error messages, and UI states.
    /// </remarks>
    public MainViewModel(Window? parentWindow = null)
    {
        _parentWindow = parentWindow;
        if (_parentWindow == null)
        {
            // Default logic if no parent window is provided
            AdvancedOptions = new AdvancedOptionsViewModel();
            Processes = new ProcessesViewModel();
        }

        AdvancedOptions = new AdvancedOptionsViewModel();
        Processes = new ProcessesViewModel();

        OpenXmlCommand = ReactiveCommand.CreateFromTask(OpenFileDialogAsync);
        SaveXmlCommand = ReactiveCommand.Create(() =>
        {
            if (!string.IsNullOrEmpty(CurrentFilePath))
            {
                SaveXml(CurrentFilePath);
            }
            else
            {
                ErrorMessage = "Error: No file has been opened or selected for saving.";
            }
        });
        SaveAsXmlCommand = ReactiveCommand.Create(SaveAsXml);
    }

    /// <summary>
    /// Opens a file dialog to allow the user to select an XML configuration file.
    /// </summary>
    /// <remarks>
    /// Displays a file picker dialog with options configured to filter XML files.
    /// On successful user selection, the selected file is processed (e.g., loaded into the application).
    /// </remarks>
    /// <returns>
    /// A task that represents the asynchronous file dialog operation. This task completes
    /// when the file dialog is closed and a file is selected, or no file is chosen.
    /// </returns>
    private async Task OpenFileDialogAsync()
    {
        {
            var options = new FilePickerOpenOptions
            {
                Title = "Select Configuration File",
                FileTypeFilter = [new FilePickerFileType("XML Files") { Patterns = ["*.xml"] }],
                AllowMultiple = false
            };

            if (_parentWindow != null)
            {
                var result = await _parentWindow.StorageProvider.OpenFilePickerAsync(options);
                if (result.Count > 0)
                {
                    var filePath = result[0].Path.LocalPath;
                    LoadXml(filePath);
                }
            }
        }
    }

    /// <summary>
    /// Loads and parses an XML file from the specified file path, populating the application's advanced options
    /// and processes with the data extracted from the XML structure.
    /// </summary>
    /// <param name="filePath">The file path of the XML file to be loaded and parsed.</param>
    /// <remarks>
    /// This method reads the XML structure of a configuration file, extracts the data relevant to advanced options
    /// and processes, and updates the application state accordingly. If any error occurs during loading or parsing,
    /// the error message is updated to reflect the issue.
    /// </remarks>
    private void LoadXml(string filePath)
    {
        try
        {
            var document = XDocument.Load(filePath);
            var root = document.Element("xSE")?.Element("PluginPreloader");

            if (root == null) return;
            // Load Advanced Options
            AdvancedOptions.OriginalLibrary = root.Element("OriginalLibrary")?.Value ?? "";
            var loadMethod = root.Element("LoadMethod");
            if (loadMethod != null)
            {
                AdvancedOptions.ImportLibraryName =
                    loadMethod.Element("ImportAddressHook")?.Element("LibraryName")?.Value ?? "";
                AdvancedOptions.ImportFunctionName =
                    loadMethod.Element("ImportAddressHook")?.Element("FunctionName")?.Value ?? "";
                AdvancedOptions.ThreadNumber =
                    loadMethod.Element("OnThreadAttach")?.Element("ThreadNumber")?.Value ?? "";
            }

            AdvancedOptions.LoadDelay = root.Element("LoadDelay")?.Value ?? "0";
            AdvancedOptions.HookDelay = root.Element("HookDelay")?.Value ?? "0";

            // Load Processes
            Processes.ProcessItems.Clear();
            var processes = root.Element("Processes");
            if (processes == null) return;
            foreach (var item in processes.Elements("Item"))
            {
                var name = item.Attribute("Name")?.Value;
                var allow = bool.TryParse(item.Attribute("Allow")?.Value, out var result) && result;
                var tooltip = item.Nodes().OfType<XComment>().FirstOrDefault()?.Value ?? "";

                if (!string.IsNullOrEmpty(name))
                {
                    Processes.ProcessItems.Add(new ProcessItemViewModel
                    {
                        Name = name,
                        Allow = allow,
                        Tooltip = tooltip
                    });
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading XML: {ex.Message}";
        }
    }

    /// <summary>
    /// Saves the current configuration data to an XML file at the specified file path.
    /// </summary>
    /// <param name="filePath">
    /// The file path where the XML configuration should be saved. If null or empty, the method will attempt to use the current file path.
    /// </param>
    /// <remarks>
    /// This method generates an XML document containing advanced options and processes data, and saves it to the specified location.
    /// If the file path is invalid or an error occurs during the saving process, an appropriate error message is set.
    /// </remarks>
    private void SaveXml(string? filePath = null)
    {
        filePath ??= CurrentFilePath; // Use CurrentFilePath if no filePath is provided

        if (string.IsNullOrEmpty(filePath))
        {
            ErrorMessage = "Error: No file path specified.";
            return;
        }

        try
        {
            var root = new XElement("Root");

            // Add advanced options to the XML
            var advancedOptions = new XElement("AdvancedOptions",
                new XElement("OriginalLibrary", AdvancedOptions.OriginalLibrary),
                new XElement("ImportLibraryName", AdvancedOptions.ImportLibraryName),
                new XElement("ImportFunctionName", AdvancedOptions.ImportFunctionName),
                new XElement("ThreadNumber", AdvancedOptions.ThreadNumber),
                new XElement("LoadDelay", AdvancedOptions.LoadDelay),
                new XElement("HookDelay", AdvancedOptions.HookDelay)
            );
            root.Add(advancedOptions);

            // Add processes to the XML
            var processes = new XElement("Processes",
                Processes.ProcessItems.Select(p => new XElement("Process",
                    new XAttribute("Name", p.Name ?? string.Empty),
                    new XAttribute("Allow", p.Allow),
                    new XComment(p.Tooltip ?? "")
                ))
            );
            root.Add(processes);

            // Save the XML document
            var document = new XDocument(root);
            document.Save(filePath);

            CurrentFilePath = filePath; // Update the current file path
            ErrorMessage = "Configuration saved successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving XML: {ex.Message}";
        }
    }

    /// <summary>
    /// Saves the current configuration as an XML file, allowing the user to specify a file name and location.
    /// </summary>
    /// <remarks>
    /// The method prompts the user with a save file dialog to select a destination and provides default options
    /// for file name and type. If a file path is selected, it saves the XML configuration to the specified file.
    /// This command handles scenarios where the save operation is canceled or fails.
    /// </remarks>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous save operation.
    /// </returns>
    private async Task SaveAsXml()
    {
        if (_parentWindow?.StorageProvider != null)
        {
            var options = new FilePickerSaveOptions
            {
                Title = "Save Configuration File",
                SuggestedFileName = "config.xml",
                FileTypeChoices = [new FilePickerFileType("XML Files") { Patterns = ["*.xml"] }]
            };

            var result = await _parentWindow.StorageProvider.SaveFilePickerAsync(options);
            if (result != null)
            {
                CurrentFilePath = result.Path.LocalPath; // Update the file path
                SaveXml(CurrentFilePath);
            }
            else
            {
                ErrorMessage = "Save operation canceled.";
            }
        }
    }
}