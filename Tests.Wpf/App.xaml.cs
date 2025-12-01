using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace Tests.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureServices((sc) =>
            {
                sc.AddSingleton<MainWindow>();
                sc.AddSingleton<MainViewModel>();
            });

        var host = hostBuilder.Build();

        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
