using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DesignTimeData;
using Tests.Wpf.DragDrop;
using Tests.Wpf.Medias;
using Tests.Wpf.Models;
using Tests.Wpf.ProgressMask;
using Tests.Wpf.Validations;

namespace Tests.Wpf;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<ViewItem> ViewItems { get; set; }

    [ObservableProperty]
    public partial string ToastMessage { get; set; } = string.Empty;

    public MainViewModel()
    {
        ViewItems =
        [
            new() { Name = nameof(PlayerView) },
            new() { Name = nameof(VisibilityView) },
            new() { Name = nameof(GridSplitterView) },
            new() { Name = nameof(CustomizedTabItemView) },
            new() { Name = nameof(MessageBoxView) },
            new() { Name = nameof(DragDropView) },
            new() { Name = nameof(ProgressMaskView) },
            new() { Name = nameof(DesignTimeDataView) },
            new() { Name = nameof(ValidationView) },
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
