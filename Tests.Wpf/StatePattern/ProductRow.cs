using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.StatePattern;

// Row model with change notification so panel commits show up in the DataGrid immediately.
// 行模型带变更通知，使面板提交的修改能立即反映到 DataGrid。
public sealed partial class ProductRow : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Category { get; set; } = string.Empty;

    [ObservableProperty]
    public partial decimal Price { get; set; }
}
