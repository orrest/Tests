using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Tests.Wpf.Controls;
using Tests.Wpf.CustomizedMessageBox;
using Tests.Wpf.DragDrop;
using Tests.Wpf.Medias;
using Tests.Wpf.Messages;
using Tests.Wpf.Models;

namespace Tests.Wpf;

public partial class MainViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial ObservableCollection<ViewItem> ViewItems { get; set; }

    [ObservableProperty]
    public partial ViewItem? SelectedView { get; set; }

    [ObservableProperty]
    public partial string ToastMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsProgressMaskShown { get; set; }

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

    [RelayCommand]
    private void ShowProgressMask()
    {
        Messenger.Send(
            new ProgressMaskMessage(visible: true, caption: "Loading..."),
            nameof(ProgressMask)
        );
    }

    [RelayCommand]
    private async Task ShowMaskThenManuallyCancelIteration()
    {
        var rand = new Random();

        using var source = new CancellationTokenSource();
        var token = source.Token;

        // begin loading
        Messenger.Send("Start loading...", nameof(MainViewModel));

        var i = 1;
        for (; i <= 10; i++)
        {
            // if cancelled
            if (token.IsCancellationRequested)
            {
                break;
            }

            // during loading
            Messenger.Send(
                new ProgressMaskMessage(
                    visible: true,
                    caption: $"During iteration [{i}]...",
                    cancellationTokenSource: source
                ),
                nameof(ProgressMask)
            );

            await Task.Delay(rand.Next(500, 1200));
        }

        // finished
        var finishMessage = token.IsCancellationRequested
            ? $"Finished by manually cancellation before [{i}]!"
            : $"{i - 1} iterations finished!";

        Messenger.Send(finishMessage, nameof(MainViewModel));

        Messenger.Send(new ProgressMaskMessage(visible: false), nameof(ProgressMask));
    }
}
