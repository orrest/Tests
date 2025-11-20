using System.Windows.Controls;

namespace Tests.Wpf.Threadings;

public partial class PlayerView : UserControl
{
    public PlayerView()
    {
        InitializeComponent();

        DataContext = new PlayerViewModel();
    }
}
