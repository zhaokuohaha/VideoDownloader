using System.Diagnostics;
using System.Runtime.InteropServices;
using VideoDownloader.Core.Services;

namespace VideoDownloader.Desktop.Services;

public class DesktopPlatformService : IPlatformService
{
    public void OpenFolder(string path)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", path);
        }
        else
        {
            Process.Start("xdg-open", path);
        }
    }

    public string GetDefaultDownloadFolder()
    {
        return Path.Combine(AppContext.BaseDirectory, "Videos");
    }

    public string GetYtDlpPath()
    {
        var exeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "yt-dlp.exe" : "yt-dlp";
        return Path.Combine(AppContext.BaseDirectory, "Assets", "core", "bin", exeName);
    }

    public string GetFfmpegPath()
    {
        var exeName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ffmpeg.exe" : "ffmpeg";
        return Path.Combine(AppContext.BaseDirectory, "Assets", "core", "bin", exeName);
    }
}
