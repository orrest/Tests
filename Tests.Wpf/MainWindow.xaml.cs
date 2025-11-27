using System.Windows;

namespace Tests.Wpf;

public partial class MainWindow : Window
{
    public static MainWindow? Instance { get; private set; }

    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();

        Instance = this;
    }
}