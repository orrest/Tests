using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Helpers;

namespace Tests.Wpf.Tree;

public partial class ChildViewModel(string name, TreeViewItemViewModel parent) 
    : TreeViewItemViewModel(name, parent)
{
    [RelayCommand]
    private void AddLeaf()
    {
        var leaf = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), this);

        AddChild(leaf);
    }
}
