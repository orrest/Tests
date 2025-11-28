using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DragDrop;
using Tests.Wpf.Medias;
using Tests.Wpf.Models;
using Tests.Wpf.ProgressMask;

namespace Tests.Wpf;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<ViewItem> ViewItems { get; set; }

    [ObservableProperty]
    public partial ViewItem? SelectedView { get; set; }

    [ObservableProperty]
    public partial string ToastMessage { get; set; } = string.Empty;

    public MainViewModel()
    {
        ViewItems =
        [
            new()
            {
                Name = nameof(PlayerView),
                Type = typeof(PlayerView),
                Instance = new PlayerView(),
            },
            new()
            {
                Name = nameof(VisibilityView),
                Type = typeof(VisibilityView),
                Instance = new VisibilityView(),
            },
            new()
            {
                Name = nameof(GridSplitterView),
                Type = typeof(GridSplitterView),
                Instance = new GridSplitterView(),
            },
            new()
            {
                Name = nameof(CustomizedTabItemView),
                Type = typeof(CustomizedTabItemView),
                Instance = new CustomizedTabItemView(),
            },
            new()
            {
                Name = nameof(MessageBoxView),
                Type = typeof(MessageBoxView),
                Instance = new MessageBoxView(),
            },
            new()
            {
                Name = nameof(DragDropView),
                Type = typeof(DragDropView),
                Instance = new DragDropView(),
            },
            new()
            {
                Name = nameof(ProgressMaskView),
                Type = typeof(ProgressMaskView),
                Instance = new ProgressMaskView(),
            },
        ];

        Messenger.Register<string, string>(
            this,
            nameof(MainViewModel),
            (r, m) =>
            {
                ToastMessage = m;
            }
            );
    }
}
