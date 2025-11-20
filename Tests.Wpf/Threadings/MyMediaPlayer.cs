using LibVLCSharp.Shared;

namespace Tests.Wpf.Threadings;

public class MyMediaPlayer : MediaPlayer
{
    protected TaskCompletionSource<bool> endReachedTask = null!;

    public MyMediaPlayer(LibVLC libVLC)
        : base(libVLC) { }

    public async Task<bool> PlayAsync(Media media)
    {
        endReachedTask = new TaskCompletionSource<bool>();

        EndReached += OnEndReached;
        EncounteredError += OnError;

        var playResult = Play(media);
        if (!playResult)
        {
            EndReached -= OnEndReached;
            EncounteredError -= OnError;

            return false;
        }

        return await endReachedTask.Task;
    }

    private void OnEndReached(object? sender, EventArgs e)
    {
        EndReached -= OnEndReached;
        EncounteredError -= OnError;
        endReachedTask.TrySetResult(true);
    }

    private void OnError(object? sender, EventArgs e)
    {
        EndReached -= OnEndReached;
        EncounteredError -= OnError;
        endReachedTask.TrySetResult(false);
    }
}
