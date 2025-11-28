using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Tests.Wpf.Models;

namespace Tests.Wpf.DesignTimeData;

public partial class DesignTimeDataViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<SampleItem> Items { get; set; }

    public DesignTimeDataViewModel()
    {
        
        Items =
        [
            new()
            {
                Id = 1,
                Name = "Tom",
                Address = "Some where in the world",
                Gender = true,
            }
        ];
    }
}
