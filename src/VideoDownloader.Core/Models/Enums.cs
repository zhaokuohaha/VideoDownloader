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

    public enum CookieSourceType
    {
        None,
        Browser,
        File,
    }

    public enum CookieBrowserType
    {
        Chrome,
        Firefox,
        Edge,
        Safari,
        Opera,
        Brave,
    }
}
