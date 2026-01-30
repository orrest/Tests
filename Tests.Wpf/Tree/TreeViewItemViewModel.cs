using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Tree;

public abstract partial class TreeViewItemViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial TreeViewItemViewModel? Parent { get; private set; }

    [ObservableProperty]
    public partial ObservableCollection<TreeViewItemViewModel> Children { get; private set; } = [];

    [ObservableProperty]
    public partial string Name { get; private set; }

    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    [ObservableProperty]
    public partial bool IsMultiSelecting { get; set; }

    [ObservableProperty]
    public partial bool IsSelected { get; set; }

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    protected TreeViewItemViewModel(string name, TreeViewItemViewModel? parent)
    {
        Name = name;
        Parent = parent;

        Parent?.Children.Add(this);
    }

    public void AddChild(TreeViewItemViewModel child)
    {
        Children.Add(child);
    }

    public void RemoveChild(TreeViewItemViewModel child)
    {
        Children.Remove(child);
    }

    public void RemoveSelf()
    {
        Parent?.RemoveChild(this);
    }

    [RelayCommand]
    private void EnterRenameState()
    {
        IsEditing = true;
    }

    [RelayCommand]
    private void ExitRenameState()
    {
        IsEditing = false;
    }

}
