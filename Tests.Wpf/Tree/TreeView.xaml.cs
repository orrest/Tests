using System.Windows.Controls;

namespace Tests.Wpf.Tree;

public partial class TreeView : UserControl
{
    public TreeView(TreeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
