using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Threading;

namespace Tests.Wpf.Snackbar;

public partial class SnackbarViewModel : ObservableObject
{
    private readonly DispatcherTimer timer = new();
    private readonly SnackbarService snackbarService;

    [ObservableProperty]
    public partial IEnumerable<InfoLevel> Levels { get; set; } =
        Enum.GetValues<InfoLevel>().Cast<InfoLevel>();

    [ObservableProperty]
    public partial InfoLevel SelectedLevel { get; set; } = InfoLevel.Info;

    [ObservableProperty]
    public partial IEnumerable<TimeSpan> TimeSpans { get; set; } =
    [
        TimeSpan.FromSeconds(3),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(6),
        TimeSpan.FromSeconds(7),
        TimeSpan.FromSeconds(8),
        TimeSpan.FromSeconds(9),
        TimeSpan.FromSeconds(10),
    ];

    [ObservableProperty]
    public partial TimeSpan SelectedSpan { get; set; }


    [ObservableProperty]
    public partial TimeSpan ElapsedSeconds { get; set; } = TimeSpan.Zero;

    

    public SnackbarViewModel(SnackbarService snackbarService)
    {
        this.snackbarService = snackbarService;

        SelectedSpan = TimeSpans.First();

        timer.Tick += Timer_Tick;
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        ElapsedSeconds = ElapsedSeconds.Add(TimeSpan.FromSeconds(1));
    }

    [RelayCommand]
    private void ShowSnackbar()
    {
        ElapsedSeconds = TimeSpan.Zero;

        snackbarService.Show(
            "Title",
            "They say there's nothing left...",
            SelectedLevel,
            SelectedSpan
        );
    }
}
