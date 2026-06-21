using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

// ReSharper disable once CheckNamespace
namespace Tests.Wpf.ControlsShowcase;

public partial class TabControlShowcaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial int SelectedTabIndex { get; set; }

    [RelayCommand]
    private void Previous()
    {
        if (SelectedTabIndex > 0)
            SelectedTabIndex--;
    }

    [RelayCommand]
    private void Next()
    {
        if (SelectedTabIndex < 2)
            SelectedTabIndex++;
    }
}
