using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orrest.Navigation;
using Tests.Wpf.Border;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DesignTimeData;
using Tests.Wpf.DragDrop;
using Tests.Wpf.Medias;
using Tests.Wpf.ModelNotification;
using Tests.Wpf.ProgressMask;
using Tests.Wpf.Snackbar;
using Tests.Wpf.Themes;
using Tests.Wpf.Tree;
using Tests.Wpf.Validations;

namespace Tests.Wpf.Extensions;

public static class DependencyInjectionExtensions
{
    public static IHostBuilder ConfigureViews(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices(
            (sc) =>
            {
                sc.AddSingleton<MainWindow>();
                sc.AddSingleton<MainViewModel>();

                sc.AddSingleton<SnackbarService>();

                // navigation
                sc.AddSingletonForNavigation(typeof(VisibilityView), typeof(VisibilityViewModel));
                sc.AddSingletonForNavigation(typeof(GridSplitterView), typeof(GridSplitterViewModel));
                sc.AddSingletonForNavigation(typeof(CustomizedTabItemView), typeof(CustomizedTabItemViewModel));
                sc.AddSingletonForNavigation(typeof(MessageBoxView), typeof(MessageBoxViewModel));
                sc.AddSingletonForNavigation(typeof(DragDropView), typeof(DragDropViewModel));
                sc.AddSingletonForNavigation(typeof(ProgressMaskView), typeof(ProgressMaskViewModel));
                sc.AddSingletonForNavigation(typeof(DesignTimeDataView), typeof(DesignTimeDataViewModel));
                sc.AddSingletonForNavigation(typeof(ValidationView), typeof(ValidationViewModel));
                sc.AddSingletonForNavigation(typeof(PlayerView), typeof(PlayerViewModel));
                sc.AddSingletonForNavigation(typeof(SnackbarView), typeof(SnackbarViewModel));
                sc.AddSingletonForNavigation(typeof(StylePriorityView), typeof(StylePriorityViewModel));
                sc.AddSingletonForNavigation(typeof(BorderView), typeof(BorderViewModel));
                sc.AddSingletonForNavigation(typeof(TreeView), typeof(TreeViewModel));

                // Model notification demo - three-layer nested scenario (StepVm → ActionVm → OptionVm)
                // ActionVm/OptionVm are created internally by StepViewModel, only StepViewModel needs registration.
                // Direct: VM 直接暴露 Model (ObservableObject); Delegate: VM 代理属性，Model 为 POCO。
                sc.AddSingleton<Tests.Wpf.ModelNotification.Direct.StepViewModel>();
                sc.AddSingleton<Tests.Wpf.ModelNotification.Delegate.StepViewModel>();
                sc.AddSingletonForNavigation(typeof(ModelNotificationView), typeof(ModelNotificationViewModel));
            }
        );
    }
}
