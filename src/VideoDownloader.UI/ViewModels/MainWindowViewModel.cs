using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using VideoDownloader.Core.Models;
using VideoDownloader.Core.Services;

namespace VideoDownloader.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string videoFolder = string.Empty;

    [ObservableProperty]
    private string url = string.Empty;

    [ObservableProperty]
    private PageType currentPage = PageType.Home;

    [ObservableProperty]
    private bool isAllSelected;

    [ObservableProperty]
    private string? batchResolution;

    [ObservableProperty]
    private bool hasVideoItems;

    private readonly INotificationService _notificationService;
    private readonly IDialogService _dialogService;
    private readonly IPlatformService _platformService;
    private readonly ISettingsService _settingsService;
    private bool _suppressSelectAllSync;

    public ObservableCollection<VideoItemViewModel> VideoItems { get; } = [];
    public ObservableCollection<string> BatchResolutions { get; } = [];
    public SettingsViewModel SettingsViewModel { get; }

    public MainWindowViewModel(
        INotificationService notificationService,
        IDialogService dialogService,
        IPlatformService platformService,
        ISettingsService settingsService)
    {
        _notificationService = notificationService;
        _dialogService = dialogService;
        _platformService = platformService;
        _settingsService = settingsService;

        var settings = settingsService.Load();
        SettingsViewModel = new SettingsViewModel(settingsService, dialogService, settings);

        ShowInfoCommand = new RelayCommand(ShowInfo);
        ShowSettingCommand = new RelayCommand(ShowSetting);
        OpenDownloadPathCommand = new RelayCommand(OpenDownloadPath);

        var folder = !string.IsNullOrEmpty(settings.DownloadFolderPath)
            ? settings.DownloadFolderPath
            : _platformService.GetDefaultDownloadFolder();
        ChangeVideoFolder(folder);
    }

    public ICommand ShowInfoCommand { get; }
    public ICommand ShowSettingCommand { get; }
    public ICommand OpenDownloadPathCommand { get; }

    public string Title { get; set; } = "摘星辰";

    partial void OnIsAllSelectedChanged(bool value)
    {
        if (_suppressSelectAllSync) return;
        foreach (var item in VideoItems)
            item.IsSelected = value;
    }

    [RelayCommand]
    private async Task QueryVideos()
    {
        if (string.IsNullOrWhiteSpace(Url)) return;

        var urls = Url.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct()
            .ToList();

        if (urls.Count == 0) return;

        VideoItems.Clear();
        HasVideoItems = false;

        var ytDlpPath = _platformService.GetYtDlpPath();
        var ytDlpOptions = YtDlpOptions.FromAppSettings(_settingsService.Load());

        foreach (var u in urls)
        {
            var item = new VideoItemViewModel(u, VideoFolder, ytDlpPath, _notificationService, ytDlpOptions);
            item.StatusChanged += UpdateBatchResolutions;
            item.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(VideoItemViewModel.IsSelected))
                    SyncSelectAllState();
            };
            VideoItems.Add(item);
        }

        HasVideoItems = true;

        var tasks = VideoItems.Select(item => item.QueryAsync());
        await Task.WhenAll(tasks);
    }

    [RelayCommand]
    private async Task DownloadSelected()
    {
        var selectedItems = VideoItems
            .Where(v => v.IsSelected && v.Status == VideoItemStatus.Ready)
            .ToList();

        if (selectedItems.Count == 0)
        {
            _notificationService.Show("提示", "请先选择要下载的视频", NotificationType.Warning);
            return;
        }

        int targetHeight = 0;
        if (!string.IsNullOrEmpty(BatchResolution))
        {
            var digits = new string(BatchResolution.Where(char.IsDigit).ToArray());
            int.TryParse(digits, out targetHeight);
        }

        var tasks = selectedItems.Select(item =>
        {
            VideoFormat? format = targetHeight > 0 ? item.FindClosestFormat(targetHeight) : null;
            return item.DownloadAsync(format);
        });

        await Task.WhenAll(tasks);
    }

    private void UpdateBatchResolutions()
    {
        var readyItems = VideoItems.Where(v => v.Status == VideoItemStatus.Ready).ToList();

        var allLabels = readyItems
            .SelectMany(v => v.QualityOptions)
            .Where(f => f.HeightPixels > 0)
            .Select(f => f.QualityLabel)
            .ToList();

        var uniqueLabels = allLabels
            .Distinct()
            .OrderByDescending(label =>
            {
                var digits = new string(label.Where(char.IsDigit).ToArray());
                return int.TryParse(digits, out var n) ? n : 0;
            })
            .ToList();

        BatchResolutions.Clear();
        foreach (var label in uniqueLabels)
            BatchResolutions.Add(label);

        if (BatchResolutions.Count > 0 && (BatchResolution == null || !BatchResolutions.Contains(BatchResolution)))
        {
            var mostCommon = allLabels
                .GroupBy(l => l)
                .OrderByDescending(g => g.Count())
                .First().Key;
            BatchResolution = mostCommon;
        }
    }

    private void SyncSelectAllState()
    {
        _suppressSelectAllSync = true;
        IsAllSelected = VideoItems.Count > 0 && VideoItems.All(v => v.IsSelected);
        _suppressSelectAllSync = false;
    }

    private void ChangeVideoFolder(string path)
    {
        if (VideoFolder != path)
            VideoFolder = path;

        if (!Directory.Exists(VideoFolder))
            Directory.CreateDirectory(VideoFolder);
    }

    private void ShowSetting()
    {
        CurrentPage = CurrentPage == PageType.Settings ? PageType.Home : PageType.Settings;
    }

    private async void ShowInfo()
    {
        await _dialogService.ShowAlertAsync("关于", "摘星辰\n" +
            "版本： 2.0.0\n" +
            "作者：zzz\n" +
            "GitHub：https://github.com/zhaokuohaha/VideoDownloader",
            "确定");
    }

    private void OpenDownloadPath()
    {
        _platformService.OpenFolder(VideoFolder);
    }
}
