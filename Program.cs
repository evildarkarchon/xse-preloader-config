using System;
using Avalonia;
using Avalonia.ReactiveUI;

namespace xse_preloader_config;

internal abstract class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    /// <summary>
    /// The entry point for the application. Initializes and starts the main application loop.
    /// </summary>
    /// <param name="args">An array of strings containing command-line arguments passed to the application.</param>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    /// <summary>
    /// Configures and initializes the Avalonia application builder, setting up platform detection, logging, fonts, and the ReactiveUI framework.
    /// </summary>
    /// <returns>An instance of <see cref="AppBuilder"/> configured for the application.</returns>
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}