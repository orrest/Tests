using System.Windows.Controls;

namespace Tests.Controls.Button;

public partial class ButtonView : UserControl
{
    public ButtonView(ButtonViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}