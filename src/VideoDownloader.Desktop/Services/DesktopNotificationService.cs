using VideoDownloader.Core.Services;

namespace VideoDownloader.Desktop.Services;

public class DesktopNotificationService : INotificationService
{
    public event Action<string, string, NotificationType>? OnNotification;

    public void Show(string title, string message, NotificationType type = NotificationType.Info)
    {
        OnNotification?.Invoke(title, message, type);
    }
}
