using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.Validations;

/// <summary>
/// Demo row model for the DataGrid annotation-validation sample.
/// 用于 DataGrid 注解验证示例的行模型。
/// </summary>
public partial class ValidationItem : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Name length must be 2-20.")]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    [Range(0, 150, ErrorMessage = "Age must be in [0, 150].")]
    public partial int Age { get; set; }

    [ObservableProperty]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public partial string Email { get; set; } = string.Empty;

    // Trigger annotation validation whenever a property changes.
    // 属性变化时触发注解验证。
    partial void OnNameChanged(string value) => ValidateProperty(value, nameof(Name));
    partial void OnAgeChanged(int value) => ValidateProperty(value, nameof(Age));
    partial void OnEmailChanged(string value) => ValidateProperty(value, nameof(Email));
}
