using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Controls;
using Tests.Wpf.Helpers;

namespace Tests.Wpf.Tree;

public partial class TreeViewModel : ObservableObject
{
    public ObservableCollection<RootViewModel> Roots { get; }

    public TreeViewModel()
    {
        var r1 = new RootViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var r2 = new RootViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var r3 = new RootViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };

        Roots = [r1, r2, r3];

        var c1 = new ChildViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var c2 = new ChildViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var c3 = new ChildViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var c4 = new ChildViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var c5 = new ChildViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };
        var c6 = new ChildViewModel() { Name = FakerHelper.INSTANCE.Lorem.Word() };

        r1.Children.Add(c1);
        r1.Children.Add(c2);

        r2.Children.Add(c3);
        r2.Children.Add(c4);
        r2.Children.Add(c5);

        r3.Children.Add(c6);

        var l1 = new LeafViewModel() { Parent = c1, Name = FakerHelper.INSTANCE.Lorem.Word() };
        var l2 = new LeafViewModel() { Parent = c1, Name = FakerHelper.INSTANCE.Lorem.Word() };
        c1.Children.Add(l1);
        c1.Children.Add(l2);

        var l3 = new LeafViewModel() { Parent = c2, Name = FakerHelper.INSTANCE.Lorem.Word() };
        c2.Children.Add(l3);

        var l4 = new LeafViewModel() { Parent = c3, Name = FakerHelper.INSTANCE.Lorem.Word() };
        c3.Children.Add(l4);

        var l5 = new LeafViewModel() { Parent = c4, Name = FakerHelper.INSTANCE.Lorem.Word() };
        c4.Children.Add(l5);

        var l6 = new LeafViewModel() { Parent = c5, Name = FakerHelper.INSTANCE.Lorem.Word() };
        var l7 = new LeafViewModel() { Parent = c5, Name = FakerHelper.INSTANCE.Lorem.Word() };
        c5.Children.Add(l6);
        c5.Children.Add(l7);

        var l8 = new LeafViewModel() { Parent = c6, Name = FakerHelper.INSTANCE.Lorem.Word() };
        c6.Children.Add(l8);
    }
}

public partial class RootViewModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    public ObservableCollection<ChildViewModel> Children { get; } = [];

    [RelayCommand]
    private void AddChild()
    {
        Children.Add(new ChildViewModel()
        {
            Name = FakerHelper.INSTANCE.Lorem.Word(),
        });
    }

}

public partial class ChildViewModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;

    public ObservableCollection<LeafViewModel> Children { get; } = [];

    [RelayCommand]
    private void AddLeaf()
    {
        Children.Add(new LeafViewModel()
        {
            Parent = this,
            Name = FakerHelper.INSTANCE.Lorem.Word(),
        });
    }
}

public partial class LeafViewModel : ObservableObject
{
    public ChildViewModel? Parent { get; set; }

    public string Name { get; set; } = string.Empty;

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
        Parent?.Children.Remove(this);
    }
}
