using System.Windows;
using Tests.Wpf.Snackbar;

namespace Tests.Wpf;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel, SnackbarService snackbarService)
    {
        InitializeComponent();

        DataContext = viewModel;

        snackbarService.SetContentControl(snackbar);
    }
}
