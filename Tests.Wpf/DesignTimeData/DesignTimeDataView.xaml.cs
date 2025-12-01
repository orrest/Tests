using System.Windows.Controls;

namespace Tests.Wpf.DesignTimeData;

public partial class DesignTimeDataView : UserControl
{
    public DesignTimeDataView(DesignTimeDataViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
