using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using VideoDownloader.Models;
using VideoDownloader.Utils;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace VideoDownloader.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private VideoInfoStatus videoInfoStatus = VideoInfoStatus.Default;

    [ObservableProperty]
    private string videoFolder;

    [ObservableProperty]
    private string url;

    [ObservableProperty]
    private string videoTitle;

    [ObservableProperty]
    private string videoThumb;

    [ObservableProperty]
    private double downloadProgress;

    [ObservableProperty]
    private bool downloadProgressVisible;

    private ISnackbarService _snackbarService;
    private IContentDialogService _contentDialogService;

    public MainWindowViewModel(ISnackbarService snackbarService, IContentDialogService contentDialogService)
    {
        ShowInfoCommand = new RelayCommand(ShowInfo);
        ShowSettingCommand = new RelayCommand(ShowSetting);
        OpenDownloadPathCommand = new RelayCommand(OpenDownloadPath);
        QueryVidesCommand = new RelayCommand(QueryVideos);
        DownloadVideoCommand = new RelayCommand<VideoFormat>(DownloadVideoAsync);
        ChangeVideoFolder(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos"));
        _snackbarService = snackbarService;
        _contentDialogService = contentDialogService;
    }

    public ICommand ShowInfoCommand { get; }
    public ICommand ShowSettingCommand { get; }
    public ICommand OpenDownloadPathCommand { get; }
    public ICommand QueryVidesCommand { get; }
    public ICommand DownloadVideoCommand { get; }

    public string Title { get; set; } = "摘星辰";

    public YtDlp YtDlp { get; set; }

    public ObservableCollection<VideoFormat> VideoFormat { get; set; } = [];

    private void ChangeVideoFolder(string path)
    {
        if (VideoFolder == null || VideoFolder != path)
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
       await _contentDialogService.ShowAlertAsync("关于", "摘星辰\n" +
           "版本： 1.0.0\n" +
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
            VideoFormat.Clear();
            YtDlp = new YtDlp (Url, VideoFolder);
            VideoTitle = await YtDlp.GetVideoTitle();
            VideoThumb = await YtDlp.GetVideoThumbnail();
            var formats = await YtDlp.GetVideoFormats();
            if (formats.Count != 0)
            {
                foreach (var format in formats)
                {
                    VideoFormat.Add(format);
                }
            }
            else
            {

            }

            if (VideoFormat.Count != 0)
            {
                VideoInfoStatus = VideoInfoStatus.Completed;
            }
            else
            {
                VideoInfoStatus = VideoInfoStatus.Error;
            }
        }
        catch(Exception ex)
        {
            VideoInfoStatus = VideoInfoStatus.Error;
        }

    }

    private async void DownloadVideoAsync(VideoFormat? format)
    {
        if (format == null || YtDlp == null)
        {
            return;
        }

        try
        {
            DownloadProgressVisible = true;
            await YtDlp.DownloadByFormat(format, progress => DownloadProgress = progress);
            _snackbarService.Show(
                   "下载完成",
                   "请打开下载文件夹查看",
                   ControlAppearance.Info,
                    new SymbolIcon(SymbolRegular.Checkmark12),
                    TimeSpan.FromSeconds(5)
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
        Process.Start("explorer.exe", VideoFolder);
    }
}
