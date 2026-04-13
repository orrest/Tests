namespace Orrest.Navigation;

public class NavigationRegistry
{
    public required string Name { get; set; }
    public required Type ViewType { get; set; }
    public required Type ViewModelType { get; set; }
}
