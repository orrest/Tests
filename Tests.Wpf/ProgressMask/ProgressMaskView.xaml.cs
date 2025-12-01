using System.Windows.Controls;

namespace Tests.Wpf.ProgressMask;

public partial class ProgressMaskView : UserControl
{
    public ProgressMaskView(ProgressMaskViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
