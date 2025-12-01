using System.Windows.Controls;

namespace Tests.Wpf.CustomizedMessageBox;

public partial class MessageBoxView : UserControl
{
    public MessageBoxView(MessageBoxViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
