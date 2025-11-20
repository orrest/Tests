using System.Windows.Controls;

namespace Tests.Wpf.Controls;

public partial class VisibilityView : UserControl
{
    public VisibilityView()
    {
        InitializeComponent();

        DataContext = new VisibilityViewModel();
    }
}
