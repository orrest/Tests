using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tests.Controls.Extensions;

namespace Tests.Controls;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Orrest.Controls.Themes.ThemeManager.Initialize(Orrest.Controls.Themes.ThemeMode.System);

        var hostBuilder = Host.CreateDefaultBuilder().ConfigureViews();

        var host = hostBuilder.Build();

        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}