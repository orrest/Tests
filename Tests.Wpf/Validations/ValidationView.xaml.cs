using System.Windows.Controls;

namespace Tests.Wpf.Validations;

public partial class ValidationView : UserControl
{
    public ValidationView()
    {
        InitializeComponent();
        DataContext = new ValidationViewModel();
    }
}
