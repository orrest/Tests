using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tests.Wpf.Controls;

namespace Tests.Wpf.CustomizedMessageBox;

public partial class MessageBoxViewModel : ObservableRecipient
{
    [RelayCommand]
    private async Task CreateOnNonUiThread()
    {
        await Task.Run(async () =>
        {
            Messenger.Send(
                $"Thread [{Environment.CurrentManagedThreadId}] is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}.",
                nameof(MainViewModel)
            );

            try
            {
                var box = new MessageBox();
                Messenger.Send(
                    $"Thread [{Environment.CurrentManagedThreadId}] show message box.",
                    nameof(MainViewModel)
                );

                var result = await box.ShowDialogAsync();
                Messenger.Send(
                    $"Thread [{Environment.CurrentManagedThreadId}] message box result {result}.",
                    nameof(MainViewModel)
                );
            }
            catch (Exception e)
            {
                Messenger.Send(e.Message, nameof(MainViewModel));
            }
        });
    }

    [RelayCommand]
    private async Task CreateOnUiThreadButSetPropertiesOnNonUiThread()
    {
        var box = await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            return new MessageBox();
        });

        await Task.Run(() =>
        {
            try
            {
                box.Title = "Create On Ui Thread";
                box.Caption = "But Set Properties On Non-Ui Thread";
            }
            catch (Exception e)
            {
                Messenger.Send(
                    $"Set properties of {nameof(MessageBox)} on thread {Environment.CurrentManagedThreadId} failed, because {e.Message}",
                    nameof(MainViewModel)
                );
            }
        });

        // can't reach
        var result = await box.ShowDialogAsync();
    }

    [RelayCommand]
    private async Task SetOnUiThreadAndContinueOnNonUiThread()
    {
        var box = await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            return new MessageBox()
            {
                Title = "Create on UI Thread",
                Caption = "Set properties on UI thread",
                ConfirmText = "OK",
                RefuseText = "Cancel",
            };
        });

        await Task.Run(async () =>
        {
            try
            {
                var r = await box.ShowDialogAsync();
                if (r == System.Windows.MessageBoxResult.OK)
                {
                    Messenger.Send("I'm OK here.", nameof(MainViewModel));
                }
                else if (r == System.Windows.MessageBoxResult.Cancel)
                {
                    Messenger.Send("Cancelled.", nameof(MainViewModel));
                }
            }
            catch (Exception)
            {
                Messenger.Send(
                    "The `ShowDialogAsync` method finally called the `ShowDialog` which also belongs to the UI thread.",
                    nameof(MainViewModel)
                );
            }
        });
    }

    [RelayCommand]
    private async Task ShowOnUiThreadAndContinueOnNonUiThread()
    {
        var result = await System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            var box = new MessageBox()
            {
                Title = "Create on UI Thread",
                Caption = "Set properties on UI thread",
                ConfirmText = "Yes",
                RefuseText = "No",
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen,
            };

            var t = box.ShowDialogAsync();

            return t;
        });

        await Task.Run(() =>
        {
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                Messenger.Send("I'm OK here.", nameof(MainViewModel));
            }
            else if (result == System.Windows.MessageBoxResult.No)
            {
                Messenger.Send("Cancelled.", nameof(MainViewModel));
            }
        });
    }
}
