using System.Windows.Controls;

namespace Tests.Wpf.Medias;

public partial class PlayerView : UserControl
{
    public PlayerView()
    {
        InitializeComponent();

        DataContext = new PlayerViewModel();
    }
}
