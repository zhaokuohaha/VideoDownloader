namespace VideoDownloader.Core.Models
{
    public enum VideoInfoStatus
    {
        Default = 0,
        Querying = 1,
        Completed = 2,
        Error = 3,
    }

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
}
