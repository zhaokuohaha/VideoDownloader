namespace VideoDownloader.Core.Models
{
    public enum PageType
    {
        Home,
        Settings,
    }

    public enum ThemeMode
    {
        System,
        Light,
        Dark,
    }

    public enum VideoItemStatus
    {
        Querying,
        Ready,
        Downloading,
        Downloaded,
        Error,
    }
}
