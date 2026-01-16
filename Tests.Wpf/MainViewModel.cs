using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Tests.Wpf.Constants;
using Tests.Wpf.Models;

namespace Tests.Wpf;

public partial class MainViewModel : ObservableRecipient
{
    private readonly IServiceProvider serviceProvider;

    [ObservableProperty]
    public partial ObservableCollection<ViewItem> ViewItems { get; set; }

    [ObservableProperty]
    public partial string ToastMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ViewItem? SelectedView { get; set; }

    [ObservableProperty]
    public partial object? ContentView { get; set; }

    public MainViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        ViewItems =
        [
            new("Audio player", "PlayerView"),
            new("Border", "BorderView"),
            new("Customized tab item", "CustomizedTabItemView"),
            new("Data validation", "ValidationView"),
            new("Design time data", "DesignTimeDataView"),
            new("Drag drop", "DragDropView"),
            new("Grid splitter", "GridSplitterView"),
            new("Message box", "MessageBoxView"),
            new("Progress", "ProgressMaskView"),
            new("Snackbar", "SnackbarView"),
            new("Style priority", "StylePriorityView"),
            new("Visibility", "VisibilityView"),
            new("Local-built Wpf Assemblies", "LocalWpfView"),
        ];

        Messenger.Register<string, string>(
            this,
            Channels.NAVIGATION,
            (r, m) =>
            {
                Navigate(m);
            }
        );

        Messenger.Register<string, string>(
            this,
            Channels.TOAST,
            (r, m) =>
            {
                ToastMessage = m;
            }
        );
    }

    private void Navigate(string viewName)
    {
        var views = this.serviceProvider.GetService<IEnumerable<ViewRegistryItem>>();
        var viewItem = views?.LastOrDefault(v => v.Name == viewName);
        if (viewItem is null)
        {
            return;
        }

        var view = this.serviceProvider.GetService(viewItem.ViewType);
        if (view is null)
        {
            return;
        }

        ContentView = view;
    }

    partial void OnSelectedViewChanged(ViewItem? value)
    {
        if (value is null)
        {
            return;
        }

        Messenger.Send(value.ViewName, Channels.NAVIGATION);
    }
}
