using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using VideoDownloader.Core.Models;
using VideoDownloader.Core.Services;
using VideoDownloader.Core.Utils;

namespace VideoDownloader.UI.ViewModels;

public partial class VideoItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string url;

    [ObservableProperty]
    private VideoItemStatus status = VideoItemStatus.Querying;

    [ObservableProperty]
    private VideoInfo? videoInfo;

    [ObservableProperty]
    private Bitmap? thumbnailBitmap;

    [ObservableProperty]
    private double downloadProgress;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private VideoFormat? selectedFormat;

    private static readonly HttpClient _httpClient = new();
    private readonly string _videoFolder;
    private readonly string _ytDlpPath;
    private readonly INotificationService _notificationService;
    private readonly YtDlpOptions? _ytDlpOptions;
    private YtDlp? _ytDlp;

    public ObservableCollection<VideoFormat> QualityOptions { get; } = [];

    public event Action? StatusChanged;

    public VideoItemViewModel(string url, string videoFolder, string ytDlpPath, INotificationService notificationService, YtDlpOptions? ytDlpOptions = null)
    {
        this.url = url;
        _videoFolder = videoFolder;
        _ytDlpPath = ytDlpPath;
        _notificationService = notificationService;
        _ytDlpOptions = ytDlpOptions;
    }

    public async Task QueryAsync()
    {
        try
        {
            Status = VideoItemStatus.Querying;
            _ytDlp = new YtDlp(Url, _videoFolder, _ytDlpPath, _ytDlpOptions);
            var info = await _ytDlp.GetVideoInfo();
            if (info == null)
            {
                Status = VideoItemStatus.Error;
                return;
            }

            VideoInfo = info;

            var videoFormats = info.Videos
                .Where(f => f.HeightPixels > 0)
                .OrderByDescending(f => f.HeightPixels)
                .ToList();

            QualityOptions.Clear();
            foreach (var f in videoFormats)
                QualityOptions.Add(f);

            SelectedFormat = QualityOptions.FirstOrDefault();

            await LoadThumbnailAsync(info.Thumbnail);
            Status = VideoItemStatus.Ready;
        }
        catch
        {
            Status = VideoItemStatus.Error;
        }
        finally
        {
            StatusChanged?.Invoke();
        }
    }

    [RelayCommand]
    private async Task Download()
    {
        await DownloadAsync();
    }

    public async Task DownloadAsync(VideoFormat? overrideFormat = null)
    {
        if (_ytDlp == null || Status == VideoItemStatus.Downloading) return;

        var format = overrideFormat ?? SelectedFormat;
        if (format?.FormatId == null) return;

        var formatStr = $"{format.FormatId}+bestaudio/{format.FormatId}";

        try
        {
            Status = VideoItemStatus.Downloading;
            DownloadProgress = 0;
            await _ytDlp.DownloadByFormat(formatStr, progress => DownloadProgress = progress);
            Status = VideoItemStatus.Downloaded;
            _notificationService.Show("下载完成", VideoInfo?.Title ?? Url, NotificationType.Success);
        }
        catch
        {
            Status = VideoItemStatus.Error;
            _notificationService.Show("下载失败", VideoInfo?.Title ?? Url, NotificationType.Error);
        }
    }

    public VideoFormat? FindClosestFormat(int targetHeight)
    {
        var formats = QualityOptions.Where(f => f.HeightPixels > 0).ToList();
        if (formats.Count == 0) return SelectedFormat;

        var exact = formats.FirstOrDefault(f => f.HeightPixels == targetHeight);
        if (exact != null) return exact;

        var lower = formats
            .Where(f => f.HeightPixels < targetHeight)
            .OrderByDescending(f => f.HeightPixels)
            .FirstOrDefault();
        if (lower != null) return lower;

        return formats
            .Where(f => f.HeightPixels > targetHeight)
            .OrderBy(f => f.HeightPixels)
            .FirstOrDefault();
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
