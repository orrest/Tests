using System.Windows.Controls;

namespace Tests.Wpf.Controls;

public partial class CustomizedTabItemView : UserControl
{
    public CustomizedTabItemView()
    {
        InitializeComponent();

        DataContext = new CustmizedTabItemViewModel();
    }
}
