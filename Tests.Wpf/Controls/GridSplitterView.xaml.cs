using System.Diagnostics;
using System.Windows.Controls;

namespace Tests.Wpf.Controls;

public partial class GridSplitterView : UserControl
{
    public GridSplitterView()
    {
        InitializeComponent();

        DataContext = new GridSplitterViewModel();
    }

    private void Hyperlink_RequestNavigate(
        object sender,
        System.Windows.Navigation.RequestNavigateEventArgs e
    )
    {
        Process.Start(
            new ProcessStartInfo { FileName = e.Uri.AbsoluteUri, UseShellExecute = true }
        );
    }
}
