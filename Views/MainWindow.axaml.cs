using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using xse_preloader_config.ViewModels;

namespace xse_preloader_config.Views;

public class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}