using System.Windows.Controls;

namespace Tests.Wpf.Controls;

public partial class CustomizedTabItemView : UserControl
{
    public CustomizedTabItemView(CustomizedTabItemViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
