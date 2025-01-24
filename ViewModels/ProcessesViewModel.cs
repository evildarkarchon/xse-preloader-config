using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using xse_preloader_config.Services;

namespace xse_preloader_config.ViewModels;


using System.Collections.ObjectModel;

public class ProcessesViewModel : ViewModelBase
{
    public ObservableCollection<ProcessItemViewModel> ProcessItems { get; set; } =
        new ObservableCollection<ProcessItemViewModel>();

    /// Loads the process data into the ViewModel, clearing any existing process items
    /// and adding new instances of ProcessItemViewModel based on the provided data.
    /// <param name="processes">The collection of ProcessData objects to load into the ViewModel.</param>
    public void LoadFromData(IEnumerable<ProcessData> processes)
    {
        ProcessItems.Clear(); // Clear existing items
        foreach (var process in processes)
        {
            ProcessItems.Add(new ProcessItemViewModel
            {
                Name = process.Name,
                Allow = process.Allow
            });
        }
    }

    /// Converts the collection of processes in the ViewModel into an XML format.
    /// Each process is represented as an "Item" element with "Name" and "Allow" attributes.
    /// <returns>An XElement containing all process data serialized into XML format.</returns>
    public XElement SaveToXml()
    {
        var processElements = ProcessItems
            .Select(item => new XElement("Item",
                new XAttribute("Name", item.Name ?? string.Empty),
                new XAttribute("Allow", item.Allow.ToString().ToLower())));

        return new XElement("Processes", processElements);
    }
}