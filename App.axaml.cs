using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using xse_preloader_config.ViewModels;
using xse_preloader_config.Views;

namespace xse_preloader_config
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Create the MainWindow
                var mainWindow = new MainWindow();

                // Pass mainWindow as the parentWindow to MainViewModel
                mainWindow.DataContext = new MainViewModel(mainWindow);

                // Assign the main window to the desktop lifetime
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}