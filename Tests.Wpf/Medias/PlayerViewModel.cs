using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Tests.Wpf.Constants;
using Tests.Wpf.Threadings;

namespace Tests.Wpf.Medias;

public sealed partial class PlayerViewModel : ObservableRecipient, IDisposable
{
    private readonly List<FileInfo> sounds;
    private readonly LibVLC vlc;

    private MediaPlayer? currentPlayer;
    private int currentIndex;

    public PlayerViewModel()
    {
        sounds = [new("./Files/sample-3s.mp3"), new("./Files/sample-6s.mp3")];
        currentIndex = 0;

        vlc = new LibVLC(enableDebugLogs: true);
    }

    public void Dispose()
    {
        currentPlayer?.Dispose();
        vlc.Dispose();
    }

    [RelayCommand]
    private void Play()
    {
        if (currentIndex >= sounds.Count)
        {
            Messenger.Send($"Index {currentIndex}, list over.", Channels.TOAST);
            return;
        }

        var sound = sounds[currentIndex];
        using var media = new Media(vlc, sound.FullName);

        currentPlayer = new MediaPlayer(vlc);
        currentPlayer?.EndReached += (s, e) =>
        {
            Messenger.Send($"Index {currentIndex}, end reached.", Channels.TOAST);

            currentIndex++;
            Play();
        };

        Messenger.Send($"Index {currentIndex}, playing...", Channels.TOAST);

        currentPlayer?.Play(media);
    }

    [RelayCommand]
    private async Task Play1()
    {
        if (currentIndex >= sounds.Count)
        {
            Messenger.Send($"Index {currentIndex}, list over.", Channels.TOAST);
            return;
        }

        var sound = sounds[currentIndex];
        using var media = new Media(vlc, sound.FullName);

        Messenger.Send($"Index {currentIndex}, playing...", Channels.TOAST);
        Messenger.Send($"On thread {Environment.CurrentManagedThreadId}, playing...", Channels.TOAST);

        currentPlayer = new MyMediaPlayer(vlc);
        bool finished = await ((MyMediaPlayer)currentPlayer).PlayAsync(media);

        Messenger.Send($"On thread {Environment.CurrentManagedThreadId}, finished...", Channels.TOAST);

        if (finished)
        {
            currentIndex++;
            await Play1();
        }
    }

    [RelayCommand]
    private void Reset()
    {
        currentPlayer?.Dispose();
        currentIndex = 0;

        Messenger.Send($"Reset index to {currentIndex}.", Channels.TOAST);
    }
}
