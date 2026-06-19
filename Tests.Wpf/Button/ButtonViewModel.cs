using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.Button;

public partial class ButtonViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool ButtonsEnabled { get; set; } = true;
}
