using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace Orrest.Controls.Window;

/// <summary>
///     定义窗口操作相关的路由命令。
/// </summary>
public static class WindowCommands
{
    public static RoutedUICommand CloseWindow { get; } = new("关闭", "CloseWindow", typeof(WindowCommands));
    public static RoutedUICommand MinimizeWindow { get; } = new("最小化", "MinimizeWindow", typeof(WindowCommands));
    public static RoutedUICommand MaximizeWindow { get; } = new("最大化", "MaximizeWindow", typeof(WindowCommands));
    public static RoutedUICommand RestoreWindow { get; } = new("还原", "RestoreWindow", typeof(WindowCommands));
}
