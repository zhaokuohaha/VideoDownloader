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
    private VideoInfo videoInfo;

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
        ChangeVideoFolder(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Videos"));
        _snackbarService = snackbarService;
        _contentDialogService = contentDialogService;
    }

    public ICommand ShowInfoCommand { get; }
    public ICommand ShowSettingCommand { get; }
    public ICommand OpenDownloadPathCommand { get; }
    public ICommand QueryVidesCommand { get; }

    public string Title { get; set; } = "摘星辰";

    public YtDlp YtDlp { get; set; }


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
           "版本： 1.2.0\n" +
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
            YtDlp = new YtDlp (Url, VideoFolder);
            var videoInfo = await YtDlp.GetVideoInfo();
            if (videoInfo == null)
            {
                VideoInfoStatus = VideoInfoStatus.Error;
            }
            else
            {
                VideoInfoStatus = VideoInfoStatus.Completed;
                VideoInfo = videoInfo!;
            }
        }
        catch(Exception ex)
        {
            VideoInfoStatus = VideoInfoStatus.Error;
        }

    }

    [RelayCommand]
    private async void OnDownloadVideo(string formatId)
    {
        // 自定义下载格式，先组装formatId
        if (string.IsNullOrEmpty(formatId))
        {
            var formatIds = VideoInfo.Formats.Where(x => x.IsSelected).Select(x => x.FormatId);
            if (!formatIds.Any())
            {
                _snackbarService.Show(
                    "下载取消",
                    "未选择任何格式",
                    ControlAppearance.Caution,
                    new SymbolIcon(SymbolRegular.Warning16),
                    TimeSpan.FromSeconds(5)
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
            _snackbarService.Show(
                   "下载完成",
                   "请打开下载文件夹查看",
                   ControlAppearance.Success,
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
