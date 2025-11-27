using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Tests.Wpf.Messages;

namespace Tests.Wpf.Controls;

public class ProgressMask : ProgressBar
{
    private CancellationTokenSource? cancellationTokenSource;
    private readonly Stopwatch stopwatch = new();

    public double ProgressBarWidth
    {
        get { return (double)GetValue(ProgressBarWidthProperty); }
        set { SetValue(ProgressBarWidthProperty, value); }
    }

    public static readonly DependencyProperty ProgressBarWidthProperty =
        DependencyProperty.Register(
            "ProgressBarWidth",
            typeof(double),
            typeof(ProgressMask),
            new PropertyMetadata(280d)
        );

    public double ProgressBarHeight
    {
        get { return (double)GetValue(ProgressBarHeightProperty); }
        set { SetValue(ProgressBarHeightProperty, value); }
    }

    public static readonly DependencyProperty ProgressBarHeightProperty =
        DependencyProperty.Register(
            "ProgressBarHeight",
            typeof(double),
            typeof(ProgressMask),
            new PropertyMetadata(10d)
        );

    public string Caption
    {
        get { return (string)GetValue(CaptionProperty); }
        set { SetValue(CaptionProperty, value); }
    }

    public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
        "Caption",
        typeof(string),
        typeof(ProgressMask),
        new PropertyMetadata(string.Empty)
    );

    public string CancellationText
    {
        get { return (string)GetValue(CancellationTextProperty); }
        set { SetValue(CancellationTextProperty, value); }
    }

    public static readonly DependencyProperty CancellationTextProperty =
        DependencyProperty.Register(
            "CancellationText",
            typeof(string),
            typeof(ProgressMask),
            new PropertyMetadata("Cancel")
        );

    public IRelayCommand CancelCommand
    {
        get { return (IRelayCommand)GetValue(CancelCommandProperty); }
    }

    public static readonly DependencyProperty CancelCommandProperty = DependencyProperty.Register(
        "CancelCommand",
        typeof(IRelayCommand),
        typeof(ProgressMask),
        new PropertyMetadata(null)
    );

    public ProgressMask()
    {
        SetValue(CancelCommandProperty, new RelayCommand(OnCancelClick));

        WeakReferenceMessenger.Default.Register<ProgressMaskMessage, string>(
            this,
            nameof(ProgressMask),
            (r, m) =>
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    cancellationTokenSource = m.CancellationTokenSource;

                    // from invisible to visible
                    if (m.Visible && !(Visibility == Visibility.Visible))
                    {
                        Visibility = Visibility.Visible;
                        stopwatch.Restart();
                    }
                    // from visible to invisible
                    else if (!m.Visible && Visibility == Visibility.Visible)
                    {
                        stopwatch.Stop();
                        var elapsed = stopwatch.ElapsedMilliseconds;
                        if (elapsed < 300)
                        {
                            await Task.Delay(300);
                        }
                        Visibility = Visibility.Collapsed;
                    }

                    if (m.Caption is not null)
                    {
                        Caption = m.Caption;
                    }
                });
            }
        );
    }

    private void OnCancelClick()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            cancellationTokenSource?.Cancel();
            Visibility = Visibility.Collapsed;
        });
    }
}
