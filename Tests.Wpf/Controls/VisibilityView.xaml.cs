using System.Windows.Controls;

namespace Tests.Wpf.Controls;

public partial class VisibilityView : UserControl
{
    public VisibilityView(VisibilityViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
