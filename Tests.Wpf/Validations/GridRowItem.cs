using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.Validations;

// Row model without per-property annotations, so the only error source is the
// row-level ValidationRule that runs when the user commits the row.
// 无属性级注解的行模型，唯一错误来源是用户提交行时求值的行级 ValidationRule。
public partial class GridRowItem : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int Price { get; set; }

    [ObservableProperty]
    public partial int DiscountPrice { get; set; }
}
