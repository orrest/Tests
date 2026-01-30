using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Wpf.Helpers;

namespace Tests.Wpf.Tree;

public partial class TreeViewModel : ObservableObject
{
    public ObservableCollection<RootViewModel> Roots { get; }

    public TreeViewModel()
    {
        var r1 = new RootViewModel(FakerHelper.INSTANCE.Lorem.Word());
        var r2 = new RootViewModel(FakerHelper.INSTANCE.Lorem.Word());
        var r3 = new RootViewModel(FakerHelper.INSTANCE.Lorem.Word());

        var c1 = new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), r1);
        var c2 = new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), r1);
        var c3 = new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), r2);
        var c4 = new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), r2);
        var c5 = new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), r2);
        var c6 = new ChildViewModel(FakerHelper.INSTANCE.Lorem.Word(), r3);
        
        var l1 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c1);
        var l2 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c1);

        var l3 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c2);

        var l4 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c3);

        var l5 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c4);

        var l6 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c5);
        var l7 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c5);

        var l8 = new LeafViewModel(FakerHelper.INSTANCE.Lorem.Word(), c6);

        Roots = [r1, r2, r3];
    }
}


