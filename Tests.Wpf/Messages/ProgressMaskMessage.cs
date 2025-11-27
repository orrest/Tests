using System.Windows;

namespace Tests.Wpf.Messages;

public class ProgressMaskMessage(
    bool visible,
    string? caption = "",
    CancellationTokenSource? cancellationTokenSource = null
)
{
    public bool Visible { get; private set; } = visible;
    public string? Caption { get; private set; } = caption;
    public CancellationTokenSource? CancellationTokenSource { get; private set; } = cancellationTokenSource;
}
