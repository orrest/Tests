using System.Windows.Controls;

namespace Tests.Wpf.Tree;

public partial class TreeView : UserControl
{
    public TreeView(TreeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void TreeView_SelectedItemChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
    {
        // if ctrl is pressed && enter this method

        var vm = e.NewValue as TreeViewItemViewModel;
        if (vm is null)
        {
            return;
        }

        //vm.IsSelected = true;

    }
}
