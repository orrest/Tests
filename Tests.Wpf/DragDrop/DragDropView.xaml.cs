using System.Windows.Controls;

namespace Tests.Wpf.DragDrop;

public partial class DragDropView : UserControl
{
    public DragDropView(DragDropViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
