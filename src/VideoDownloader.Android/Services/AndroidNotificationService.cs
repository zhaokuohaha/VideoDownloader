using VideoDownloader.Core.Services;

namespace VideoDownloader.Android.Services;

public class AndroidNotificationService : INotificationService
{
    public event Action<string, string, NotificationType>? OnNotification;

    public void Show(string title, string message, NotificationType type = NotificationType.Info)
    {
        OnNotification?.Invoke(title, message, type);
    }
}
