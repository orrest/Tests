using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Tests.Wpf.SelectedItem;

public partial class SelectedItemViewModel : ObservableObject
{
    private const int MaxLogEntries = 100;

    public ObservableCollection<PersonRow> Rows { get; } =
    [
        new() { Name = "Alice", Age = 30, City = "Beijing" },
        new() { Name = "Bob", Age = 25, City = "Shanghai" },
        new() { Name = "Charlie", Age = 35, City = "Shenzhen" },
        new() { Name = "Diana", Age = 28, City = "Hangzhou" },
        new() { Name = "Evan", Age = 40, City = "Guangzhou" },
    ];

    // Enum sources for the option ComboBoxes.
    // 选项 ComboBox 的枚举数据源。
    public IReadOnlyList<DataGridSelectionUnit> SelectionUnits { get; } = Enum.GetValues<DataGridSelectionUnit>();

    public IReadOnlyList<DataGridSelectionMode> SelectionModes { get; } = Enum.GetValues<DataGridSelectionMode>();

    [ObservableProperty]
    public partial DataGridSelectionUnit SelectionUnit { get; set; } = DataGridSelectionUnit.FullRow;

    [ObservableProperty]
    public partial DataGridSelectionMode SelectionMode { get; set; } = DataGridSelectionMode.Single;

    // Readouts mirrored from the DataGrid on every selection change.
    // 每次选中变化后从 DataGrid 同步过来的读数。
    [ObservableProperty]
    public partial object? SelectedItem { get; set; }

    [ObservableProperty]
    public partial int SelectedIndex { get; set; } = -1;

    [ObservableProperty]
    public partial int SelectedCellCount { get; set; }

    [ObservableProperty]
    public partial string SelectedItemsText { get; set; } = "-";

    [ObservableProperty]
    public partial string CurrentCellText { get; set; } = "-";

    [ObservableProperty]
    public partial ObservableCollection<string> EventLog { get; set; } = [];

    // Single sync point for SelectedCellsChanged, covering rows and cells in every SelectionUnit.
    // SelectedCellsChanged 的唯一同步点，覆盖所有 SelectionUnit 下的行选中与单元格选中。
    public void UpdateSelection(DataGrid grid)
    {
        SelectedItem = grid.SelectedItem;
        SelectedIndex = grid.SelectedIndex;
        SelectedCellCount = grid.SelectedCells.Count;
        var items = grid.SelectedItems.Cast<object>().Select(static item => item.ToString());
        SelectedItemsText = grid.SelectedItems.Count == 0 ? "-" : $"({grid.SelectedItems.Count}) {string.Join(", ", items)}";
        var cells = grid.SelectedCells.Select(static info => $"{info.Item} @ {info.Column?.Header ?? "-"}");
        var selectedCellsText = grid.SelectedCells.Count == 0 ? "-" : $"({grid.SelectedCells.Count}) {string.Join(", ", cells)}";
        var cell = grid.CurrentCell;
        CurrentCellText = cell.IsValid ? $"{cell.Item} @ {cell.Column?.Header ?? "-"}" : "-";
        Log($"selectedItems={SelectedItemsText}, selectedCells={selectedCellsText}, selectedItem={grid.SelectedItem ?? "(null)"}, selectedIndex={grid.SelectedIndex}");
    }

    public void Log(string message)
    {
        EventLog.Insert(0, $"[{DateTime.Now:HH:mm:ss.fff}] {message}");
        while (EventLog.Count > MaxLogEntries)
        {
            EventLog.RemoveAt(EventLog.Count - 1);
        }
    }

    partial void OnSelectionUnitChanged(DataGridSelectionUnit value)
    {
        Log($"SelectionUnit -> {value}");
    }

    partial void OnSelectionModeChanged(DataGridSelectionMode value)
    {
        Log($"SelectionMode -> {value}");
    }
}
