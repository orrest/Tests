using System.Windows.Controls;

namespace Tests.Wpf.ModelNotification;

public partial class ModelNotificationView : UserControl
{
    public ModelNotificationView(ModelNotificationViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
