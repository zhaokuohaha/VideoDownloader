using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using VideoDownloader.Core.Services;

namespace VideoDownloader.UI.Controls;

public partial class NotificationHost : UserControl
{
    private DispatcherTimer? _hideTimer;

    public NotificationHost()
    {
        InitializeComponent();
    }

    public void Show(string title, string message, NotificationType type)
    {
        Dispatcher.UIThread.Post(() =>
        {
            TitleText.Text = title;
            MessageText.Text = message;

            NotificationBorder.Background = type switch
            {
                NotificationType.Success => new SolidColorBrush(Color.FromRgb(46, 125, 50)),
                NotificationType.Warning => new SolidColorBrush(Color.FromRgb(237, 108, 2)),
                NotificationType.Error => new SolidColorBrush(Color.FromRgb(211, 47, 47)),
                _ => new SolidColorBrush(Color.FromRgb(50, 50, 50)),
            };

            NotificationBorder.IsVisible = true;

            _hideTimer?.Stop();
            _hideTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _hideTimer.Tick += (_, _) =>
            {
                NotificationBorder.IsVisible = false;
                _hideTimer.Stop();
            };
            _hideTimer.Start();
        });
    }
}
