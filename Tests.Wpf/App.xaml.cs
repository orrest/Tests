using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tests.Wpf.Extensions;

namespace Tests.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var hostBuilder = Host.CreateDefaultBuilder().ConfigureViews();

        var host = hostBuilder.Build();

        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
