using System.Windows.Controls;

namespace Tests.Wpf.Snackbar;

public partial class SnackbarView : UserControl
{
    public SnackbarView(SnackbarViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
