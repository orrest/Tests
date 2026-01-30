using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Controls;
using Tests.Wpf.Helpers;

namespace Tests.Wpf.Tree;

public partial class LeafViewModel(string name, TreeViewItemViewModel parent) 
    : TreeViewItemViewModel(name, parent)
{
    [RelayCommand]
    private async Task Open()
    {
        var box = new MessageBox()
        {
            Title = $"Leaf - {Name}",
            Caption = FakerHelper.INSTANCE.Lorem.Sentence(),
            ConfirmText = "OK",
        };

        await box.ShowDialogAsync();
    }

    [RelayCommand]
    private void Delete()
    {
        RemoveSelf();
    }
}
