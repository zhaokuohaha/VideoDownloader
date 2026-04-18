using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using VideoDownloader.Core.Services;
using VideoDownloader.Desktop.Services;
using VideoDownloader.UI;
using VideoDownloader.UI.ViewModels;

namespace VideoDownloader.Desktop;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App.Services = ConfigureServices();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Platform services
        services.AddSingleton<IPlatformService, DesktopPlatformService>();
        services.AddSingleton<DesktopNotificationService>();
        services.AddSingleton<INotificationService>(sp => sp.GetRequiredService<DesktopNotificationService>());
        services.AddSingleton<IDialogService, DesktopDialogService>();
        services.AddSingleton<ISettingsService, DesktopSettingsService>();

        // ViewModels
        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
