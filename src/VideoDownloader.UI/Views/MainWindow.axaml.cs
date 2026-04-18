using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using VideoDownloader.Core.Services;

namespace VideoDownloader.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Wire up notification service to the NotificationHost control
        var notifyService = App.Services?.GetService<INotificationService>();
        if (notifyService != null)
        {
            notifyService.OnNotification += (title, message, type) =>
            {
                NotificationHost?.Show(title, message, type);
            };
        }
    }
}
