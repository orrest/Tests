using Tests.Wpf.Models;

namespace Tests.Wpf;

public static class DesignTimeDataSource
{
    public static List<SampleItem> Samples =
    [
        new()
        {
            Id = 1,
            Name = "Tom",
            Address = "Home",
            Gender = false,
        },
    ];
}
