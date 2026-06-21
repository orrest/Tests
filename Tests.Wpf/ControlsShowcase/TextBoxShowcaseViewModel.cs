using CommunityToolkit.Mvvm.ComponentModel;

// ReSharper disable once CheckNamespace
namespace Tests.Wpf.ControlsShowcase;

public partial class TextBoxShowcaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string InitialText { get; set; } = "Hello World";

    [ObservableProperty]
    public partial string MultiLineText { get; set; } = "";

    [ObservableProperty]
    public partial string BoundText { get; set; } = "";
}
