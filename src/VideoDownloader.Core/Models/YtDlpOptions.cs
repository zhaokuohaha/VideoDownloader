namespace VideoDownloader.Core.Models;

public class YtDlpOptions
{
    public string? ProxyUrl { get; set; }
    public string? RateLimit { get; set; }
    public int ConcurrentFragments { get; set; } = 1;
    public int Retries { get; set; } = 10;
    public int? SocketTimeout { get; set; }
    public CookieSourceType CookieSourceType { get; set; }
    public CookieBrowserType CookieBrowserType { get; set; }
    public string? CookieFilePath { get; set; }
    public string? UserAgent { get; set; }
    public string? Referer { get; set; }

    public static YtDlpOptions FromAppSettings(AppSettings s) => new()
    {
        ProxyUrl = s.ProxyUrl,
        RateLimit = s.RateLimit,
        ConcurrentFragments = s.ConcurrentFragments,
        Retries = s.Retries,
        SocketTimeout = s.SocketTimeout,
        CookieSourceType = s.CookieSourceType,
        CookieBrowserType = s.CookieBrowserType,
        CookieFilePath = s.CookieFilePath,
        UserAgent = s.UserAgent,
        Referer = s.Referer,
    };
}
