using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VideoDownloader.Core.Models;
using VideoDownloader.Core.Services;

namespace VideoDownloader.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private ThemeMode themeMode;

    [ObservableProperty]
    private string downloadFolderPath = string.Empty;

    [ObservableProperty]
    private string proxyUrl = string.Empty;

    private readonly ISettingsService _settingsService;
    private readonly IDialogService _dialogService;
    private bool _isLoading;

    public SettingsViewModel(ISettingsService settingsService, IDialogService dialogService, AppSettings settings)
    {
        _settingsService = settingsService;
        _dialogService = dialogService;

        _isLoading = true;
        ThemeMode = settings.ThemeMode;
        DownloadFolderPath = settings.DownloadFolderPath ?? string.Empty;
        ProxyUrl = settings.ProxyUrl ?? string.Empty;
        _isLoading = false;
    }

    partial void OnThemeModeChanged(ThemeMode value)
    {
        ApplyTheme(value);
        SaveSettings();
    }

    partial void OnDownloadFolderPathChanged(string value)
    {
        SaveSettings();
    }

    partial void OnProxyUrlChanged(string value)
    {
        SaveSettings();
    }

    [RelayCommand]
    private async Task PickFolder()
    {
        var path = await _dialogService.PickFolderAsync("选择下载文件夹");
        if (!string.IsNullOrEmpty(path))
        {
            DownloadFolderPath = path;
        }
    }

    public static void ApplyTheme(ThemeMode mode)
    {
        if (Application.Current is not { } app) return;

        app.RequestedThemeVariant = mode switch
        {
            ThemeMode.Light => ThemeVariant.Light,
            ThemeMode.Dark => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }

    private void SaveSettings()
    {
        if (_isLoading) return;

        _settingsService.Save(new AppSettings
        {
            ThemeMode = ThemeMode,
            DownloadFolderPath = DownloadFolderPath,
            ProxyUrl = ProxyUrl,
        });
    }
}
