using System.Windows.Controls;

namespace Tests.Controls.Themes;

public partial class ThemeSwitcherView : UserControl
{
    public ThemeSwitcherView(ThemeSwitcherViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}