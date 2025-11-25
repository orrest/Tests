using Bogus;

namespace Tests.Wpf.DragDrop;

public class DragDropItem
{
    public int Id { get; set; }
    public string Address { get; set; } = null!;
    public DateTime Date { get; set; }

    public static ICollection<DragDropItem> Generate(int n)
    {
        var faker = new Faker<DragDropItem>()
            .RuleFor(d => d.Id, f => f.Random.Int())
            .RuleFor(d => d.Address, f => f.Address.StreetAddress())
            .RuleFor(d => d.Date, f => f.Date.Past());

        var items = faker.Generate(n);

        return items;
    }
}
