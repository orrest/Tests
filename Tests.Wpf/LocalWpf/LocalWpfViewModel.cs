using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Tests.Wpf.Models;

namespace Tests.Wpf.LocalWpf;

public partial class LocalWpfViewModel : ObservableObject
{
    [ObservableProperty]
    public partial ObservableCollection<Person> Persons { get; set; }

    public LocalWpfViewModel()
    {
        Persons = [new() { Age = 1 }, new() { Age = 2 }, new() { Age = 3 }];
    }
}
