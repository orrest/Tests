namespace Tests.Wpf.SelectedItem;

// Plain row model: selection behavior is unrelated to change notification.
// 纯净行模型：选中行为与属性变更通知无关。
public sealed class PersonRow
{
    public string Name { get; set; } = string.Empty;

    public int Age { get; set; }

    public string City { get; set; } = string.Empty;

    // Nicer display for SelectedItem / CurrentCell readouts.
    // 让 SelectedItem / CurrentCell 读数更可读。
    public override string ToString()
    {
        return $"{Name} ({Age}, {City})";
    }
}
