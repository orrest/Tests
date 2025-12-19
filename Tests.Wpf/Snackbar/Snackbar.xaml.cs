using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Snackbar;

public partial class Snackbar : UserControl
{
    private readonly CancellationTokenSource cancellationSource = new();

    public SnackbarRecord Record
    {
        get { return (SnackbarRecord)GetValue(RecordProperty); }
        set { SetValue(RecordProperty, value); }
    }

    public static readonly DependencyProperty RecordProperty = DependencyProperty.Register(
        "Record",
        typeof(SnackbarRecord),
        typeof(Snackbar),
        new PropertyMetadata(null)
    );

    public IRelayCommand CloseButtonCommand => (IRelayCommand)GetValue(CloseButtonCommandProperty);

    public static readonly DependencyProperty CloseButtonCommandProperty =
        DependencyProperty.Register(
            nameof(CloseButtonCommand),
            typeof(IRelayCommand),
            typeof(Snackbar),
            new PropertyMetadata(null)
        );

    public Snackbar()
    {
        InitializeComponent();
        
        SetValue(CloseButtonCommandProperty, new RelayCommand<object>(_ => cancellationSource.Cancel()));
    }

    public async Task Show()
    {
        VisualStateManager.GoToElementState(this, "Shown", true);

        try
        {
            await Task.Delay(Record.Timeout, cancellationSource.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }
    }

    public async Task Close()
    {
        var tcs = new TaskCompletionSource();

        var sb = Hidden.Storyboard;
        sb.Completed += (_, _) =>
        {
            tcs.SetResult();
        };

        VisualStateManager.GoToElementState(this, "Hidden", true);

        cancellationSource.Dispose();

        await tcs.Task;
    }
}
