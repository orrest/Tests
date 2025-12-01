using System.Windows.Controls;

namespace Tests.Wpf.Validations;

public partial class ValidationView : UserControl
{
    public ValidationView(ValidationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
