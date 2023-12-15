using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VideoDownloader.Models;
using VideoDownloader.Utils;
using Wpf.Ui.Controls;
using Wpf.Ui.Mvvm.Services;

namespace VideoDownloader.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            ShowInfoCommand = new RelayCommand(ShowInfo);
            ShowSettingCommand = new RelayCommand(ShowSetting);
            QueryVidesCommand = new RelayCommand(QueryVideos);
        }

        public ICommand ShowInfoCommand { get; }
        public ICommand ShowSettingCommand { get; }
        public ICommand QueryVidesCommand { get; }

        public string Title { get; set; } = "摘星辰";

        [ObservableProperty]
        private string url = "https://v.youku.com/v_show/id_XNDQ0OTY2ODQ1Ng==.html";

        [ObservableProperty]
        private string videoTitle;

        [ObservableProperty]
        private string videoThumb;

        public YtDlp YtDlp { get; set; }

        public List<VideoFormat> VideoFormat { get; set; } = new List<VideoFormat>();

        private void ChangeTheme()
        {
            Wpf.Ui.Appearance.Theme.Apply(
              Wpf.Ui.Appearance.ThemeType.Light,     // Theme type
              Wpf.Ui.Appearance.BackgroundType.Mica, // Background type
              true                                   // Whether to change accents automatically
            );
        }

        private void ShowSetting()
        {
            var dialog = new DialogService();
        }

        private void ShowInfo()
        {
            var messageBox = new MessageBox
            {
                Title = "关于",
                Content = "摘星辰\n版本：1.0.0",
                ButtonLeftAppearance = Wpf.Ui.Common.ControlAppearance.Transparent,
                ButtonRightName = "确定"
            };

            messageBox.ShowDialog();
        }

        private async void QueryVideos()
        {
            if (string.IsNullOrEmpty(Url))
            {
                return;
            }

            YtDlp = new YtDlp (Url, null);
            VideoTitle = await YtDlp.GetVideoTitle();
            VideoThumb = await YtDlp.GetVideoThumbnail();
            VideoFormat = await YtDlp.GetVideoFormats();
        }
    }
}
