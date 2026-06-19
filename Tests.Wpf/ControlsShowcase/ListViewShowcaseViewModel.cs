using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.ControlsShowcase;

public partial class ListViewShowcaseViewModel : ObservableObject
{
    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial ContactItem? SelectedContact { get; set; }

    public ObservableCollection<ContactItem> Contacts { get; } =
    [
        new("Alice Johnson"),
        new("Bob Smith"),
        new("Carol White"),
        new("Dave Brown"),
        new("Eve Davis"),
    ];

    public ObservableCollection<CategoryItem> Categories { get; } =
    [
        new("Technology"),
        new("Science"),
        new("Art"),
        new("Music"),
        new("Sports"),
        new("Health"),
    ];
}

public class ContactItem
{
    public string Name { get; }

    public ContactItem(string name)
    {
        Name = name;
    }
}

public class CategoryItem
{
    public string Label { get; }

    public CategoryItem(string label)
    {
        Label = label;
    }
}
