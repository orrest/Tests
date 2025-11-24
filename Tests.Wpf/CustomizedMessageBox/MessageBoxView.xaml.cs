using System.Windows.Controls;

namespace Tests.Wpf.CustomizedMessageBox;

public partial class MessageBoxView : UserControl
{
    public MessageBoxView()
    {
        InitializeComponent();
        DataContext = new MessageBoxViewModel();
    }
}
