using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Snackbar;

public partial class Snackbar : UserControl
{
    public event EventHandler? Closed;

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
        
        Loaded += (_, _) => VisualStateManager.GoToElementState(this, "Shown", true);

        SetValue(CloseButtonCommandProperty, new RelayCommand<object>(_ => Close()));
    }

    public void Close()
    {
        var sb = Hidden.Storyboard;
        sb.Completed += (_, _) => Closed?.Invoke(this, EventArgs.Empty);

        VisualStateManager.GoToElementState(this, "Hidden", true);
    }
}
