using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Constants;
using Tests.Wpf.Helpers;

namespace Tests.Wpf.Tree;

public partial class RootViewModel(string name) : TreeViewItemViewModel(name, null)
{
    [RelayCommand]
    private void AddChild()
    {
        AddChild(new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), this));
    }

    [RelayCommand]
    private void AfterRename()
    {
        Messenger.Send($"[{nameof(RootViewModel)} - {Name}] Do something before exit rename state.", Channels.TOAST);

        // IsEditing is a two-way binding, so can omit the disable,
        // it will always be called inside the EditableText control.
    }
}
