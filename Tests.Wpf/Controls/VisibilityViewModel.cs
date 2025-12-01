using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Constants;

namespace Tests.Wpf.Controls;

public partial class VisibilityViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsVisible { get; set; }

    [RelayCommand]
    private void ChangeVisibility()
    {
        IsVisible = !IsVisible;
        Messenger.Send($"Visible: {IsVisible}", Channels.TOAST);
    }
}
