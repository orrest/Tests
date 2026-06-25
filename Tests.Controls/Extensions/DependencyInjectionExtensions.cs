using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orrest.Navigation;
using Tests.Controls.Button;
using Tests.Controls.ControlsShowcase;
using Tests.Controls.Themes;

namespace Tests.Controls.Extensions;

public static class DependencyInjectionExtensions
{
    public static IHostBuilder ConfigureViews(this IHostBuilder hostBuilder)
    {
        return hostBuilder.ConfigureServices(
            (sc) =>
            {
                sc.AddSingleton<MainWindow>();
                sc.AddSingleton<MainViewModel>();

                // navigation
                sc.AddSingletonForNavigation(typeof(ButtonView), typeof(ButtonViewModel));
                sc.AddSingletonForNavigation(typeof(CheckBoxShowcase), typeof(CheckBoxShowcaseViewModel));
                sc.AddSingletonForNavigation(typeof(ListViewShowcase), typeof(ListViewShowcaseViewModel));
                sc.AddSingletonForNavigation(typeof(ScrollBarShowcase), typeof(ScrollBarShowcaseViewModel));
                sc.AddSingletonForNavigation(typeof(TabControlShowcase), typeof(TabControlShowcaseViewModel));
                sc.AddSingletonForNavigation(typeof(TextBoxShowcase), typeof(TextBoxShowcaseViewModel));
                sc.AddSingletonForNavigation(typeof(ThemeSwitcherView), typeof(ThemeSwitcherViewModel));
            }
        );
    }
}