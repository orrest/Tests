using System.Windows.Controls;

namespace Tests.Wpf.Button;

public partial class ButtonView : UserControl
{
    public ButtonView(ButtonViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}
