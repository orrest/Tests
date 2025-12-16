using System.Windows.Media;

namespace Tests.Wpf.Snackbar;

public record SnackbarRecord(
    string Title,
    string Message,
    string Icon,
    SolidColorBrush Foreground,
    SolidColorBrush ContentForeground,
    SolidColorBrush Background,
    TimeSpan Timeout
);
