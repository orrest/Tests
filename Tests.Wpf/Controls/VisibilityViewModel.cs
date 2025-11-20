using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Controls;

public partial class VisibilityViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsVisible { get; set; }

    [RelayCommand]
    private void ChangeVisibility()
    {
        IsVisible = !IsVisible;
        Messenger.Send($"Visible: {IsVisible}", nameof(MainViewModel));
    }
}
