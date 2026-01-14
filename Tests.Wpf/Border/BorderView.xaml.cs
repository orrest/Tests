using System.Windows.Controls;

namespace Tests.Wpf.Border;

public partial class BorderView : UserControl
{
    public BorderView(BorderViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
