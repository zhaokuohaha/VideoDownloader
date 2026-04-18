using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using VideoDownloader.Core.Services;
using VideoDownloader.UI.ViewModels;
using VideoDownloader.UI.Views;

namespace VideoDownloader.UI;

public partial class App : Application
{
    public static IServiceProvider? Services { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = Services!.GetRequiredService<MainWindowViewModel>();
            SettingsViewModel.ApplyTheme(vm.SettingsViewModel.ThemeMode);
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            var vm = Services!.GetRequiredService<MainWindowViewModel>();
            SettingsViewModel.ApplyTheme(vm.SettingsViewModel.ThemeMode);
            singleView.MainView = new HomeView
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
