using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Net.Http;
using System.Windows.Input;
using VideoDownloader.Core.Models;
using VideoDownloader.Core.Services;
using VideoDownloader.Core.Utils;

namespace VideoDownloader.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private VideoInfoStatus videoInfoStatus = VideoInfoStatus.Default;

    [ObservableProperty]
    private string videoFolder = string.Empty;

    [ObservableProperty]
    private string url = string.Empty;

    [ObservableProperty]
    private VideoInfo? videoInfo;

    [ObservableProperty]
    private double downloadProgress;

    [ObservableProperty]
    private bool downloadProgressVisible;

    [ObservableProperty]
    private Bitmap? thumbnailBitmap;

    private static readonly HttpClient _httpClient = new();
    private readonly INotificationService _notificationService;
    private readonly IDialogService _dialogService;
    private readonly IPlatformService _platformService;

    public MainWindowViewModel(
        INotificationService notificationService,
        IDialogService dialogService,
        IPlatformService platformService)
    {
        _notificationService = notificationService;
        _dialogService = dialogService;
        _platformService = platformService;

        ShowInfoCommand = new RelayCommand(ShowInfo);
        ShowSettingCommand = new RelayCommand(ShowSetting);
        OpenDownloadPathCommand = new RelayCommand(OpenDownloadPath);
        QueryVidesCommand = new RelayCommand(QueryVideos);
        ChangeVideoFolder(_platformService.GetDefaultDownloadFolder());
    }

    public ICommand ShowInfoCommand { get; }
    public ICommand ShowSettingCommand { get; }
    public ICommand OpenDownloadPathCommand { get; }
    public ICommand QueryVidesCommand { get; }

    public string Title { get; set; } = "摘星辰";

    public YtDlp? YtDlp { get; set; }

    private void ChangeVideoFolder(string path)
    {
        if (VideoFolder != path)
        {
            VideoFolder = path;
        }

        if (!Directory.Exists(VideoFolder))
        {
            Directory.CreateDirectory(VideoFolder);
        }
    }

    private void ShowSetting()
    {
    }

    private async void ShowInfo()
    {
        await _dialogService.ShowAlertAsync("关于", "摘星辰\n" +
            "版本： 2.0.0\n" +
            "作者：zzz\n" +
            "GitHub：https://github.com/zhaokuohaha/VideoDownloader",
            "确定");
    }

    private async void QueryVideos()
    {
        if (string.IsNullOrEmpty(Url))
        {
            return;
        }

        try
        {
            VideoInfoStatus = VideoInfoStatus.Querying;
            YtDlp = new YtDlp(Url, VideoFolder, _platformService.GetYtDlpPath());
            var info = await YtDlp.GetVideoInfo();
            if (info == null)
            {
                VideoInfoStatus = VideoInfoStatus.Error;
            }
            else
            {
                VideoInfoStatus = VideoInfoStatus.Completed;
                VideoInfo = info;
                await LoadThumbnailAsync(info.Thumbnail);
            }
        }
        catch (Exception)
        {
            VideoInfoStatus = VideoInfoStatus.Error;
        }
    }

    [RelayCommand]
    private async Task OnDownloadVideo(string? formatId)
    {
        if (VideoInfo == null || YtDlp == null) return;

        // 自定义下载格式，先组装formatId
        if (string.IsNullOrEmpty(formatId))
        {
            var formatIds = VideoInfo.Formats.Where(x => x.IsSelected).Select(x => x.FormatId);
            if (!formatIds.Any())
            {
                _notificationService.Show(
                    "下载取消",
                    "未选择任何格式",
                    NotificationType.Warning
                );
                return;
            }
            formatId = string.Join("+", formatIds);
        }

        // 下载
        try
        {
            DownloadProgressVisible = true;
            await YtDlp.DownloadByFormat(formatId, progress => DownloadProgress = progress);
            _notificationService.Show(
                "下载完成",
                "请打开下载文件夹查看",
                NotificationType.Success
            );
        }
        finally
        {
            DownloadProgressVisible = false;
            DownloadProgress = 0;
        }
    }

    private void OpenDownloadPath()
    {
        _platformService.OpenFolder(VideoFolder);
    }

    private async Task LoadThumbnailAsync(string? url)
    {
        if (string.IsNullOrEmpty(url)) return;
        try
        {
            var bytes = await _httpClient.GetByteArrayAsync(url);
            using var stream = new MemoryStream(bytes);
            ThumbnailBitmap = new Bitmap(stream);
        }
        catch
        {
            ThumbnailBitmap = null;
        }
    }
}
