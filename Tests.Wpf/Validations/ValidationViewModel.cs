using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

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
}
