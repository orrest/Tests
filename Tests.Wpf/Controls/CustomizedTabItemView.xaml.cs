using System.Windows.Controls;

namespace Tests.Wpf.Controls;

public partial class CustomizedTabItemView : UserControl
{
    public CustomizedTabItemView(CustmizedTabItemViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
