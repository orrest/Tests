using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Wpf.Models;
using Tests.Wpf.Threadings;

namespace Tests.Wpf;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<ViewItem> ViewItems { get; set; }

    [ObservableProperty]
    public partial ViewItem? SelectedView { get; set; }

    public MainViewModel()
    {
        ViewItems = [new() { Name = nameof(PlayerView), Type = typeof(PlayerView), Instance = new PlayerView() }];
    }
}
