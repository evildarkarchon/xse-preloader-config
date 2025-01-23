using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using ReactiveUI;

namespace xse_preloader_config.ViewModels;

public class ProcessesViewModel : ReactiveObject
{
    public ObservableCollection<ProcessItemViewModel> ProcessItems { get; set; } = new();

    public void LoadFromXml(XElement root)
    {
        var processes = root.Element("Processes");
        if (processes == null) return;

        foreach (var process in processes.Elements("Process"))
        {
            var name = process.Attribute("Name")?.Value;
            var allow = bool.TryParse(process.Attribute("Allow")?.Value, out var result) && result;
            var tooltip = process.Nodes().OfType<XComment>().FirstOrDefault()?.Value;

            if (!string.IsNullOrEmpty(name))
            {
                ProcessItems.Add(new ProcessItemViewModel
                {
                    Name = name,
                    Allow = allow,
                    Tooltip = tooltip
                });
            }
        }
    }
}