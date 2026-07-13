using System.Windows;
using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace Orrest.Controls.Window;

/// <summary>
///     为 Window 自动注册窗口操作命令的附加属性。
/// </summary>
public static class WindowBehavior
{
    /// <summary>
    ///     当设为 True 时，自动为 Window 注册 CloseWindow、MinimizeWindow、MaximizeWindow、RestoreWindow 的 CommandBinding。
    /// </summary>
    public static readonly DependencyProperty AttachCommandBindingsProperty =
        DependencyProperty.RegisterAttached(
            "AttachCommandBindings",
            typeof(bool),
            typeof(WindowBehavior),
            new PropertyMetadata(false, OnAttachCommandBindingsChanged));

    public static bool GetAttachCommandBindings(DependencyObject obj)
    {
        return (bool)obj.GetValue(AttachCommandBindingsProperty);
    }

    public static void SetAttachCommandBindings(DependencyObject obj, bool value)
    {
        obj.SetValue(AttachCommandBindingsProperty, value);
    }

    private static void OnAttachCommandBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not System.Windows.Window window) return;

        if ((bool)e.NewValue)
        {
            window.CommandBindings.Add(new CommandBinding(WindowCommands.CloseWindow, OnCloseWindow));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.MinimizeWindow, OnMinimizeWindow));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.MaximizeWindow, OnMaximizeWindow));
            window.CommandBindings.Add(new CommandBinding(WindowCommands.RestoreWindow, OnRestoreWindow));
        }
    }

    private static void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is System.Windows.Window window)
            window.Close();
    }

    private static void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is System.Windows.Window window)
            window.WindowState = WindowState.Minimized;
    }

    private static void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is System.Windows.Window window)
            window.WindowState = WindowState.Maximized;
    }

    private static void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
    {
        if (sender is System.Windows.Window window)
            window.WindowState = WindowState.Normal;
    }
}
