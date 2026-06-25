using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Controls.Button;

public partial class ButtonViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool ButtonsEnabled { get; set; } = true;
}