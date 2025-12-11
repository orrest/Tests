namespace Tests.Wpf.Models;

public class ViewRegistryItem
{
    public required string Name { get; set; }
    public required Type ViewType { get; set; }
    public required Type ViewModelType { get; set; }
}
