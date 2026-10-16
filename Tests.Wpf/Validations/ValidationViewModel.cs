using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Validations;

public partial class ValidationViewModel : ObservableValidator
{
    [ObservableProperty]
    [Range(21, 100, ErrorMessage = "Out of range [21, 100]")]
    [GreatherThan(nameof(MinAge))]
    public partial int Age { get; set; }

    partial void OnAgeChanged(int value)
    {
        ValidateProperty(value, nameof(Age));
    }

    [ObservableProperty]
    public partial int MinAge { get; set; } = 25;

    /// <summary>
    /// Items bound to the DataGrid for annotation-driven validation.
    /// 绑定到 DataGrid 的项集合，由注解驱动验证。
    /// </summary>
    public ObservableCollection<ValidationItem> Items { get; } = [];

    // Rows bound to the DataGrid for row-level validation (RowValidationRules).
    // 绑定到 DataGrid 的行集合，演示行级验证（RowValidationRules）。
    public ObservableCollection<GridRowItem> RowItems { get; } = [];

    public ValidationViewModel()
    {
        Items.Add(new ValidationItem { Name = "Alice", Age = 30, Email = "alice@example.com" });
        Items.Add(new ValidationItem { Name = "Bob", Age = 17, Email = "bob@example.com" });
        Items.Add(new ValidationItem { Name = "Charlie", Age = 200, Email = "charlie@example.com" });
        Items.Add(new ValidationItem { Name = "D", Age = 25, Email = "d-bad-email" });

        // The middle row violates the row rule (DiscountPrice >= Price);
        // the error shows once that row is committed.
        // 中间行违反行级规则（DiscountPrice >= Price），提交该行后才会显示错误。
        RowItems.Add(new GridRowItem { Name = "Pen", Price = 10, DiscountPrice = 8 });
        RowItems.Add(new GridRowItem { Name = "Notebook", Price = 25, DiscountPrice = 30 });
        RowItems.Add(new GridRowItem { Name = "Eraser", Price = 5, DiscountPrice = 3 });
    }

    /// <summary>
    /// Removes the specified row from the DataGrid.
    /// 从 DataGrid 中删除指定行。
    /// </summary>
    [RelayCommand]
    private void Remove(ValidationItem item)
    {
        Items.Remove(item);
    }
}
