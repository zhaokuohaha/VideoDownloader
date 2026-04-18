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

    [ObservableProperty]
    private string rateLimit = string.Empty;

    [ObservableProperty]
    private int concurrentFragments = 1;

    [ObservableProperty]
    private int retries = 10;

    [ObservableProperty]
    private decimal? socketTimeout;

    [ObservableProperty]
    private CookieSourceType cookieSourceType;

    [ObservableProperty]
    private CookieBrowserType cookieBrowserType;

    [ObservableProperty]
    private string cookieFilePath = string.Empty;

    [ObservableProperty]
    private string userAgent = string.Empty;

    [ObservableProperty]
    private string referer = string.Empty;

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
        RateLimit = settings.RateLimit ?? string.Empty;
        ConcurrentFragments = settings.ConcurrentFragments;
        Retries = settings.Retries;
        SocketTimeout = settings.SocketTimeout.HasValue ? settings.SocketTimeout.Value : null;
        CookieSourceType = settings.CookieSourceType;
        CookieBrowserType = settings.CookieBrowserType;
        CookieFilePath = settings.CookieFilePath ?? string.Empty;
        UserAgent = settings.UserAgent ?? string.Empty;
        Referer = settings.Referer ?? string.Empty;
        _isLoading = false;
    }

    partial void OnThemeModeChanged(ThemeMode value)
    {
        ApplyTheme(value);
        SaveSettings();
    }

    partial void OnDownloadFolderPathChanged(string value) => SaveSettings();
    partial void OnProxyUrlChanged(string value) => SaveSettings();
    partial void OnRateLimitChanged(string value) => SaveSettings();
    partial void OnConcurrentFragmentsChanged(int value) => SaveSettings();
    partial void OnRetriesChanged(int value) => SaveSettings();
    partial void OnSocketTimeoutChanged(decimal? value) => SaveSettings();
    partial void OnCookieSourceTypeChanged(CookieSourceType value) => SaveSettings();
    partial void OnCookieBrowserTypeChanged(CookieBrowserType value) => SaveSettings();
    partial void OnCookieFilePathChanged(string value) => SaveSettings();
    partial void OnUserAgentChanged(string value) => SaveSettings();
    partial void OnRefererChanged(string value) => SaveSettings();

    [RelayCommand]
    private async Task PickFolder()
    {
        var path = await _dialogService.PickFolderAsync("选择下载文件夹");
        if (!string.IsNullOrEmpty(path))
        {
            DownloadFolderPath = path;
        }
    }

    [RelayCommand]
    private async Task PickCookieFile()
    {
        var path = await _dialogService.PickFileAsync("选择 Cookie 文件");
        if (!string.IsNullOrEmpty(path))
        {
            CookieFilePath = path;
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
            RateLimit = string.IsNullOrWhiteSpace(RateLimit) ? null : RateLimit,
            ConcurrentFragments = ConcurrentFragments,
            Retries = Retries,
            SocketTimeout = SocketTimeout.HasValue ? (int)SocketTimeout.Value : null,
            CookieSourceType = CookieSourceType,
            CookieBrowserType = CookieBrowserType,
            CookieFilePath = string.IsNullOrWhiteSpace(CookieFilePath) ? null : CookieFilePath,
            UserAgent = string.IsNullOrWhiteSpace(UserAgent) ? null : UserAgent,
            Referer = string.IsNullOrWhiteSpace(Referer) ? null : Referer,
        });
    }
}
