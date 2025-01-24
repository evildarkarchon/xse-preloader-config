using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Controls;
using ReactiveUI;
using Avalonia.Platform.Storage; // Required for OpenFilePickerAsync
using System.Collections.Generic;
using xse_preloader_config.Services;

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
    public string SaveFilePath { get; set; }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private string _errorMessage = null!;
    public AdvancedOptionsViewModel AdvancedOptions { get; set; }
    public ProcessesViewModel Processes { get; set; }

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
    public MainViewModel(Window parentWindow)
    {
        _parentWindow = parentWindow;

        OpenXmlCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            await OpenXmlWithFilePickerAsync();
        });
        SaveXmlCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (!string.IsNullOrEmpty(CurrentFilePath))
            {
                await SaveXmlAsync(CurrentFilePath); // Use CurrentFilePath here
            }
            else
            {
                // Handle the case when no file is currently open
                ErrorMessage = "No file is currently open to save.";
            }
        });
    }

    /// <summary>
    /// Asynchronously opens and processes an XML configuration file.
    /// </summary>
    /// <remarks>
    /// This method attempts to parse an XML file from the provided file path and loads
    /// its contents into various components of the view model, such as advanced options
    /// and process configurations. If an error occurs during the operation, an error message
    /// is generated and stored.
    /// </remarks>
    /// <param name="filePath">
    /// The path to the XML file that needs to be opened and processed.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation of opening and loading the XML file.
    /// </returns>
    public async Task OpenXmlAsync(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                ErrorMessage = "No file path provided.";
                return;
            }

            CurrentFilePath = filePath;
            var parser = new XmlParser();

            // Use the asynchronous ParseAsync method
            var parsedData = await parser.ParseAsync(filePath);

            // Populate the ViewModels with parsed data
            AdvancedOptions.LoadFromData(parsedData);
            Processes.LoadFromData(parsedData.Processes);

            ErrorMessage = "File loaded successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error while opening XML: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Opens the file picker dialog to select an XML file and loads it.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task OpenXmlWithFilePickerAsync()
    {
        try
        {
            if (_parentWindow?.StorageProvider == null)
            {
                ErrorMessage = "Storage provider is not available.";
                return;
            }

            // Show file picker dialog for selecting XML files
            var result = await _parentWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open XML File",
                AllowMultiple = false, // We allow only a single file to be selected
                FileTypeFilter = new List<FilePickerFileType>
                {
                    FilePickerFileTypes.TextPlain, // Optionally, allow text files too
                    new("XML Files")
                    {
                        Patterns = ["*.xml"] // File extensions to filter
                    }
                }
            });

            if (result == null || result.Count == 0)
            {
                ErrorMessage = "No file selected.";
                return;
            }

            // Get the selected file path
            var selectedFile = result[0];
            var filePath = selectedFile.Path.LocalPath;

            // Ensure a file path is available and valid
            if (string.IsNullOrWhiteSpace(filePath))
            {
                ErrorMessage = "Invalid file selected.";
                return;
            }

            // Use the OpenXmlAsync method to load the selected file
            await OpenXmlAsync(filePath);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error while selecting or opening the file: {ex.Message}";
        }
    }

    /// <summary>
    /// Saves the application's configuration data as an XML file to the specified file path.
    /// </summary>
    /// <param name="filePath">
    /// The destination file path where the XML file should be saved. If the path is null or empty, the method will use the current file path.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation to save the XML file. Sets an appropriate error message if an exception occurs during the process.
    /// </returns>
    /// <remarks>
    /// The method consolidates configuration data from advanced options and process-related settings into an XML document and writes it to the specified file.
    /// It handles any errors that occur during the file-saving operation and sets a corresponding error message.
    /// </remarks>
    public async Task SaveXmlAsync(string filePath)
    {
        try
        {
            // Combine the XML from AdvancedOptions and Processes
            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("xSE",
                    AdvancedOptions.SaveToXml(),
                    Processes.SaveToXml()
                )
            );

            // Save the XML to the specified file
            await Task.Run(() => document.Save(filePath));

            ErrorMessage = "File saved successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error while saving XML: {ex.Message}";
        }
    }
}