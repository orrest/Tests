using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tests.Wpf.Snackbar;

public class SnackbarService
{
    private readonly Queue<SnackbarRecord> queue = [];

    private ContentControl? contentControl;

    public void SetContentControl(ContentControl contentControl)
    {
        this.contentControl = contentControl;
    }

    public void Show(string title, string message, InfoLevel infoLevel, TimeSpan timeout)
    {
        SnackbarRecord record = PrepareProperties(title, message, infoLevel, timeout);

        Application.Current.Dispatcher.Invoke(async () =>
        {
            Console.WriteLine($"Enqueue snackbar: thread id {Environment.CurrentManagedThreadId}");

            queue.Enqueue(record);

            // there's already a snackbar running
            if (queue.Count > 1)
            {
                return;
            }

            Console.WriteLine(
                $"Queue - {queue.Count}: thread id {Environment.CurrentManagedThreadId}"
            );

            while (queue.Count > 0)
            {
                Console.WriteLine($"Show snackbar: thread id {Environment.CurrentManagedThreadId}");

                var r = queue.Peek();
                await ShowSnackbar(r);
                queue.Dequeue();
            }
        });
    }

    private static SnackbarRecord PrepareProperties(
        string title,
        string message,
        InfoLevel infoLevel,
        TimeSpan timeout
    )
    {
        string icon;
        SolidColorBrush foreground = new((Color)ColorConverter.ConvertFromString("#FFFFFF"));
        SolidColorBrush contentForeground = new(
            (Color)ColorConverter.ConvertFromString("#87FFFFFF")
        );
        SolidColorBrush background;

        switch (infoLevel)
        {
            case InfoLevel.Info:
            {
                icon = "\uE946";
                background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#03A9F4")
                );
                break;
            }
            case InfoLevel.Warning:
            {
                icon = "\uEA39";
                background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#FF9800")
                );
                break;
            }
            case InfoLevel.Success:
            {
                icon = "\uE930";
                background = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString("#4CAF50")
                );
                break;
            }
            default:
                throw new ApplicationException();
        }

        var record = new SnackbarRecord(
            title,
            message,
            icon,
            foreground,
            contentForeground,
            background,
            timeout
        );

        return record;
    }

    private async Task ShowSnackbar(SnackbarRecord record)
    {
        var snackbar = new Snackbar();
        snackbar.SetCurrentValue(Snackbar.RecordProperty, record);

        if (contentControl is null)
        {
            throw new ApplicationException($"SnackbarService not has snackbar ContentControl set.");
        }

        contentControl.Content = snackbar;
        await snackbar.Show();
        await snackbar.Close();
        contentControl.Content = null;
    }
}
