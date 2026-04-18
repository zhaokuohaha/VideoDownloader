using Android.App;
using Android.Runtime;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;
using VideoDownloader.Android.Services;
using VideoDownloader.Core.Services;
using VideoDownloader.UI;
using VideoDownloader.UI.ViewModels;

namespace VideoDownloader.Android;

[Application]
public class MainApplication(nint javaReference, JniHandleOwnership transfer)
    : AvaloniaAndroidApplication<App>(javaReference, transfer)
{
    public override void OnCreate()
    {
        App.Services = ConfigureServices();
        base.OnCreate();
    }

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IPlatformService, AndroidPlatformService>();
        services.AddSingleton<INotificationService, AndroidNotificationService>();
        services.AddSingleton<IDialogService, AndroidDialogService>();
        services.AddSingleton<ISettingsService, AndroidSettingsService>();
        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }
}
