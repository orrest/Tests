using System.Windows.Controls;

namespace Tests.Wpf.StatePattern;

public partial class StatePatternView : UserControl
{
    public StatePatternView(StatePatternViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
