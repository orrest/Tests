using System.Windows.Controls;

namespace Tests.Wpf.SelectedItem;

public partial class SelectedItemView : UserControl
{
    public SelectedItemView(SelectedItemViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    // SelectedCellsChanged is the one event DataGrid raises for both row and cell selection,
    // so it is the correct sync point across every SelectionUnit/SelectionMode combination.
    // SelectedCellsChanged 是 DataGrid 对行选中与单元格选中统一触发的唯一事件，
    // 适合作为所有 SelectionUnit/SelectionMode 组合下的同步点。
    private void OnMainGridSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
    {
        if (sender is DataGrid grid && DataContext is SelectedItemViewModel viewModel)
        {
            viewModel.UpdateSelection(grid);
        }
    }
}
