namespace VideoDownloader.Core.Models
{
    public class AppSettings
    {
        public string? DownloadFolderPath { get; set; }
        public string? ProxyUrl { get; set; }
        public ThemeMode ThemeMode { get; set; } = ThemeMode.System;
    }
}
