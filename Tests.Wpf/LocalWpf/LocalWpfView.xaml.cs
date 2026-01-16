using System.Windows.Controls;

namespace Tests.Wpf.LocalWpf;

public partial class LocalWpfView : UserControl
{
    public LocalWpfView(LocalWpfViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
