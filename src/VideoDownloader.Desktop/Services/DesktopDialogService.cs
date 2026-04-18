using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using VideoDownloader.Core.Services;

namespace VideoDownloader.Desktop.Services;

public class DesktopDialogService : IDialogService
{
    public async Task ShowAlertAsync(string title, string content, string closeButtonText)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 16,
                Children =
                {
                    new TextBlock
                    {
                        Text = content,
                        TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                        FontSize = 14,
                    },
                    new Button
                    {
                        Content = closeButtonText,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        MinWidth = 80,
                    }
                }
            }
        };

        var button = ((StackPanel)dialog.Content).Children[1] as Button;
        button!.Click += (_, _) => dialog.Close();

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is not null)
        {
            await dialog.ShowDialog(desktop.MainWindow);
        }
    }

    public async Task<string?> PickFolderAsync(string title)
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow?.StorageProvider is { } storageProvider)
        {
            var result = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
            });

            if (result.Count > 0)
            {
                return result[0].Path.LocalPath;
            }
        }

        return null;
    }
}
