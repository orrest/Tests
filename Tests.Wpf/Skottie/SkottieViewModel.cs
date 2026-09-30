using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Tests.Wpf.Skottie;

public partial class SkottieViewModel : ObservableObject
{
    public ObservableCollection<LottieClip> Clips { get; } =
    [
        new() { Title = "Bouncing ball", FileName = "lottie_bouncing_ball.json" },
        new() { Title = "Rotating square", FileName = "lottie_rotating_square.json" },
        new() { Title = "Writer cat", FileName = "writer_cat.json" },
    ];

    [ObservableProperty]
    public partial LottieClip? SelectedClip { get; set; }

    [ObservableProperty]
    public partial bool IsPlaying { get; set; } = true;

    [ObservableProperty]
    public partial bool IsLooping { get; set; } = true;

    [ObservableProperty]
    public partial double Speed { get; set; } = 1.0;

    // Written by the view after parsing; parsing and the render loop are view-side concerns.
    // 由视图解析后写入；解析与渲染循环属于视图侧职责。
    [ObservableProperty]
    public partial string InfoText { get; set; } = "-";

    // Raised by the Stop command so the view can rewind its playback clock to zero.
    // Stop 命令触发该事件，让视图把播放时钟归零。
    public event EventHandler? Stopped;

    public SkottieViewModel()
    {
        SelectedClip = Clips[0];
    }

    [RelayCommand]
    private void PlayPause()
    {
        IsPlaying = !IsPlaying;
    }

    [RelayCommand]
    private void Stop()
    {
        IsPlaying = false;
        Stopped?.Invoke(this, EventArgs.Empty);
    }
}
