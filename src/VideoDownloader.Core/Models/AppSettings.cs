namespace VideoDownloader.Core.Models
{
    public class AppSettings
    {
        public string? DownloadFolderPath { get; set; }
        public string? ProxyUrl { get; set; }
        public ThemeMode ThemeMode { get; set; } = ThemeMode.System;
        public string? RateLimit { get; set; }
        public int ConcurrentFragments { get; set; } = 1;
        public int Retries { get; set; } = 10;
        public int? SocketTimeout { get; set; }
        public CookieSourceType CookieSourceType { get; set; } = CookieSourceType.None;
        public CookieBrowserType CookieBrowserType { get; set; } = CookieBrowserType.Chrome;
        public string? CookieFilePath { get; set; }
        public string? UserAgent { get; set; }
        public string? Referer { get; set; }
    }
}
