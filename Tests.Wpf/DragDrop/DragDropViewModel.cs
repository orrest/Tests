using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Tests.Wpf.DragDrop;

public partial class DragDropViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<DragDropItem> Items1 { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DragDropItem> Items2 { get; set; }

    [ObservableProperty]
    public partial DragDropHandler DropHandler { get; set; }

    public DragDropViewModel()
    {
        Items1 = new (DragDropItem.Generate(5));
        Items2 = new (DragDropItem.Generate(10));

        DropHandler = new (Items1);
    }
}
