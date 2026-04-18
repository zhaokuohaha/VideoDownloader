using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using VideoDownloader.Core.Services;

namespace VideoDownloader.Android.Services;

public class AndroidDialogService : IDialogService
{
    public Task ShowAlertAsync(string title, string content, string closeButtonText)
    {
        // On Android, use a simple approach via Avalonia window
        // For MVP, this is a no-op placeholder; can be enhanced with native Android dialogs later
        return Task.CompletedTask;
    }
}
