using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Tests.Wpf.Constants;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DesignTimeData;
using Tests.Wpf.DragDrop;
using Tests.Wpf.Messages;
using Tests.Wpf.Models;
using Tests.Wpf.ProgressMask;
using Tests.Wpf.Validations;

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
            new() { Name = "Visibility", View = ViewName.VISIBILITY },
            new() { Name = "Grid Splitter", View = ViewName.GRID_SPLLITER },
            new() { Name = "Customized TabItem", View = ViewName.CUSTOMIZED_TAB_ITEM },
            new() { Name = "Customized MessageBox", View = ViewName.MESSAGE_BOX },
            new() { Name = "DragDrop", View = ViewName.DRAG_DROP },
            new() { Name = "ProgressMask", View = ViewName.PROGRESS },
            new() { Name = nameof(DesignTimeDataView), View = ViewName.DESIGN_DATA },
            new() { Name = "Validation", View = ViewName.VALIDATION },
        ];

        Messenger.Register<NavigationMessage, string>(
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

    private void Navigate(NavigationMessage m)
    {
        var view = m.View;

        object? instance = null;
        switch (view)
        {
            case ViewName.VISIBILITY:
            {
                instance = this.serviceProvider.GetRequiredService<VisibilityView>();
                break;
            }

            case ViewName.GRID_SPLLITER:
            {
                instance = this.serviceProvider.GetRequiredService<GridSplitterView>();
                break;
            }
            case ViewName.CUSTOMIZED_TAB_ITEM:
            {
                instance = this.serviceProvider.GetRequiredService<CustomizedTabItemView>();
                break;
            }
            case ViewName.MESSAGE_BOX:
            {
                instance = this.serviceProvider.GetRequiredService<MessageBoxView>();
                break;
            }
            case ViewName.DRAG_DROP:
            {
                instance = this.serviceProvider.GetRequiredService<DragDropView>();
                break;
            }
            case ViewName.PROGRESS:
            {
                instance = this.serviceProvider.GetRequiredService<ProgressMaskView>();
                break;
            }
            case ViewName.DESIGN_DATA:
            {
                instance = this.serviceProvider.GetRequiredService<DesignTimeDataView>();
                break;
            }
            case ViewName.VALIDATION:
            {
                instance = this.serviceProvider.GetRequiredService<ValidationView>();
                break;
            }
        }

        ContentView = instance;
    }

    partial void OnSelectedViewChanged(ViewItem? value)
    {
        if (value is null)
        {
            return;
        }

        Messenger.Send(new NavigationMessage() { View = value.View }, Channels.NAVIGATION);
    }
}
