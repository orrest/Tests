using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Controls.Models;

namespace Tests.Controls.ControlsShowcase;

public partial class ScrollBarShowcaseViewModel : ObservableObject
{
    public ObservableCollection<string> VerticalItems { get; } =
    [
        "Inbox", "Starred", "Sent", "Drafts", "Spam", "Trash",
        "Archive", "Important", "Personal", "Work",
        "Family", "Friends", "Travel", "Finance",
        "Health", "Shopping", "Social", "News",
        "Entertainment", "Education", "Sports",
        "Music", "Movies", "Books", "Games",
        "Technology", "Business", "Marketing",
        "Design", "Photography", "Cooking",
    ];

    public ObservableCollection<string> HorizontalItems { get; } =
    [
        "Alpha", "Beta", "Gamma", "Delta", "Epsilon", "Zeta", "Eta", "Theta",
    ];

    public ObservableCollection<string> GridItems { get; } =
    [
        "Cell 1", "Cell 2", "Cell 3", "Cell 4", "Cell 5", "Cell 6", "Cell 7", "Cell 8",
        "Cell 9", "Cell 10", "Cell 11", "Cell 12", "Cell 13", "Cell 14", "Cell 15", "Cell 16",
        "Cell 17", "Cell 18", "Cell 19", "Cell 20",
    ];

    public ObservableCollection<ContactItem> Contacts { get; } =
    [
        new("Alice Johnson"), new("Bob Smith"), new("Carol White"),
        new("Dave Brown"), new("Eve Davis"), new("Frank Miller"),
        new("Grace Lee"), new("Henry Wilson"), new("Irene Taylor"),
        new("Jack Anderson"), new("Kate Thomas"), new("Leo Jackson"),
    ];
}