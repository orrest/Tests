using System.ComponentModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Controls;

public partial class MessageBox : Window
{
    public const string CONFIRM = nameof(CONFIRM);
    public const string REFUSE = nameof(REFUSE);
    public const string CANCEL = nameof(CANCEL);

    protected TaskCompletionSource<MessageBoxResult>? TaskCompletionSource { get; set; }

    public string ConfirmText
    {
        get { return (string)GetValue(ConfirmTextProperty); }
        set { SetValue(ConfirmTextProperty, value); }
    }

    public static readonly DependencyProperty ConfirmTextProperty = DependencyProperty.Register(
        "ConfirmText",
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(string.Empty)
    );

    public string RefuseText
    {
        get { return (string)GetValue(RefuseTextProperty); }
        set { SetValue(RefuseTextProperty, value); }
    }

    public static readonly DependencyProperty RefuseTextProperty = DependencyProperty.Register(
        "RefuseText",
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(string.Empty)
    );

    public string CancelText
    {
        get { return (string)GetValue(CancelTextProperty); }
        set { SetValue(CancelTextProperty, value); }
    }

    public static readonly DependencyProperty CancelTextProperty = DependencyProperty.Register(
        "CancelText",
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(string.Empty)
    );

    public string Caption
    {
        get { return (string)GetValue(CaptionProperty); }
        set { SetValue(CaptionProperty, value); }
    }

    public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
        "Caption",
        typeof(string),
        typeof(MessageBox),
        new PropertyMetadata(string.Empty)
    );

    public IRelayCommand ButtonCommand
    {
        get { return (IRelayCommand)GetValue(ButtonCommandProperty); }
    }

    public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register(
        "ButtonCommand",
        typeof(IRelayCommand),
        typeof(MessageBox),
        new PropertyMetadata(null)
    );

    public MessageBox()
    {
        InitializeComponent();

        Topmost = true;
        MaxWidth = 450;
        DataContext = this;
        SizeToContent = SizeToContent.WidthAndHeight;

        SetValue(ButtonCommandProperty, new RelayCommand<string>(OnButtonClick!));
    }

    private void OnButtonClick(string buttonName)
    {
        var result = buttonName switch
        {
            CONFIRM => MessageBoxResult.Yes,
            REFUSE => MessageBoxResult.No,
            CANCEL => MessageBoxResult.Cancel,
            _ => throw new ApplicationException(),
        };

        _ = TaskCompletionSource?.TrySetResult(result);

        base.Close();
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        if (e.Cancel)
        {
            return;
        }

        _ = TaskCompletionSource?.TrySetResult(MessageBoxResult.Cancel);
    }

    public async Task<MessageBoxResult> ShowDialogAsync(
        CancellationToken cancellationToken = default
    )
    {
        TaskCompletionSource = new TaskCompletionSource<MessageBoxResult>();

        CancellationTokenRegistration tokenRegistration = cancellationToken.Register(
            o => TaskCompletionSource.TrySetCanceled((CancellationToken)o!),
            cancellationToken
        );

        try
        {
            base.ShowDialog();

            return await TaskCompletionSource.Task;
        }
        finally
        {
#if NET6_0_OR_GREATER
            await tokenRegistration.DisposeAsync();
#else
            tokenRegistration.Dispose();
#endif
        }
    }
}
