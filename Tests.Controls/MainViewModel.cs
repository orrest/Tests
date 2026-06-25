using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Orrest.Navigation;
using Tests.Controls.Constants;

namespace Tests.Controls;

public partial class MainViewModel : ObservableRecipient
{
    private readonly IServiceProvider serviceProvider;

    [ObservableProperty]
    public partial ObservableCollection<NavigationItem> ViewItems { get; set; }

    [ObservableProperty]
    public partial string ToastMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial NavigationItem? SelectedView { get; set; }

    [ObservableProperty]
    public partial object? ContentView { get; set; }

    public MainViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        ViewItems =
        [
            new("Button styles", "ButtonView"),
            new("CheckBox", "CheckBoxShowcase"),
            new("ListView", "ListViewShowcase"),
            new("ScrollBar", "ScrollBarShowcase"),
            new("TabControl", "TabControlShowcase"),
            new("TextBox", "TextBoxShowcase"),
            new("Theme switcher", "ThemeSwitcherView"),
        ];

        Messenger.Register<string, string>(
            this,
            Channels.NAVIGATION,
            (r, m) =>
            {
                Navigate(m);
            }
        );

        Messenger.Register<string, string>(
            this,
            Channels.TOAST,
            (r, m) =>
            {
                ToastMessage = m;
            }
        );
    }

    private void Navigate(string viewName)
    {
        var views = this.serviceProvider.GetService<IEnumerable<NavigationRegistry>>();
        var viewItem = views?.LastOrDefault(v => v.Name == viewName);
        if (viewItem is null)
        {
            return;
        }

        var view = this.serviceProvider.GetService(viewItem.ViewType);
        if (view is null)
        {
            return;
        }

        ContentView = view;
    }

    partial void OnSelectedViewChanged(NavigationItem? value)
    {
        if (value is null)
        {
            return;
        }

        Messenger.Send(value.ViewName, Channels.NAVIGATION);
    }
}