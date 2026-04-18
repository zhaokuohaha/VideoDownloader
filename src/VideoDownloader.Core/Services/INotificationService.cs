namespace VideoDownloader.Core.Services
{
    public enum NotificationType
    {
        Success,
        Warning,
        Error,
        Info
    }

    public interface INotificationService
    {
        event Action<string, string, NotificationType>? OnNotification;
        void Show(string title, string message, NotificationType type = NotificationType.Info);
    }
}
