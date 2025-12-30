using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DesignTimeData;
using Tests.Wpf.DragDrop;
using Tests.Wpf.Medias;
using Tests.Wpf.Models;
using Tests.Wpf.ProgressMask;
using Tests.Wpf.Snackbar;
using Tests.Wpf.Themes;
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
            }
        );
    }

    public static void AddSingletonForNavigation(this IServiceCollection sc, Type view, Type viewModel)
    {
        var viewItem = new ViewRegistryItem()
        {
            Name = view.Name,
            ViewType = view,
            ViewModelType = viewModel
        };

        sc.AddSingleton(viewItem);

        sc.AddSingleton(view);
        sc.AddSingleton(viewModel);
    }
}
