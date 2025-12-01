using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Constants;
using Tests.Wpf.Messages;

namespace Tests.Wpf.ProgressMask;

public partial class ProgressMaskViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsProgressMaskShown { get; set; }

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
        Messenger.Send("Start loading...", Channels.TOAST);

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

        Messenger.Send(finishMessage, Channels.TOAST);

        Messenger.Send(new ProgressMaskMessage(visible: false), nameof(ProgressMask));
    }
}
