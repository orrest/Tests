using CommunityToolkit.Mvvm.ComponentModel;

// ReSharper disable once CheckNamespace
namespace Tests.Controls.ControlsShowcase;

public partial class CheckBoxShowcaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsChecked { get; set; }

    [ObservableProperty]
    public partial bool IsChecked2 { get; set; }

    [ObservableProperty]
    public partial bool CanToggle { get; set; } = true;
}