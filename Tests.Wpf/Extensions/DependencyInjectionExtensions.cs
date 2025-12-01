using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DesignTimeData;
using Tests.Wpf.DragDrop;
using Tests.Wpf.ProgressMask;
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

                // navigation
                sc.AddTransient<VisibilityView>();
                sc.AddTransient<VisibilityViewModel>();
                sc.AddTransient<GridSplitterView>();
                sc.AddTransient<GridSplitterViewModel>();
                sc.AddTransient<CustomizedTabItemView>();
                sc.AddTransient<CustmizedTabItemViewModel>();
                sc.AddTransient<MessageBoxView>();
                sc.AddTransient<MessageBoxViewModel>();
                sc.AddTransient<DragDropView>();
                sc.AddTransient<DragDropViewModel>();
                sc.AddTransient<ProgressMaskView>();
                sc.AddTransient<ProgressMaskViewModel>();
                sc.AddTransient<DesignTimeDataView>();
                sc.AddTransient<DesignTimeDataViewModel>();
                sc.AddTransient<ValidationView>();
                sc.AddTransient<ValidationViewModel>();
            }
        );
    }
}
