using Microsoft.Extensions.DependencyInjection;

namespace Orrest.Navigation;

public static class MsDiExtension
{
    public static void AddSingletonForNavigation(this IServiceCollection sc, Type view, Type viewModel)
    {
        var registry = new NavigationRegistry()
        {
            Name = view.Name,
            ViewType = view,
            ViewModelType = viewModel
        };

        sc.AddSingleton(registry);

        sc.AddSingleton(view);
        sc.AddSingleton(viewModel);
    }

    public static void AddTransientForNavigation(this IServiceCollection sc, Type view, Type viewModel)
    {
        var registry = new NavigationRegistry()
        {
            Name = view.Name,
            ViewType = view,
            ViewModelType = viewModel
        };

        sc.AddSingleton(registry);

        sc.AddTransient(view);
        sc.AddTransient(viewModel);
    }

    public static void AddTransientViewSingletonViewModelForNavigation(this IServiceCollection sc, Type view, Type viewModel)
    {
        var registry = new NavigationRegistry()
        {
            Name = view.Name,
            ViewType = view,
            ViewModelType = viewModel
        };

        sc.AddSingleton(registry);

        sc.AddTransient(view);
        sc.AddSingleton(viewModel);
    }

    public static void AddScopedForNavigation(this IServiceCollection sc, Type view, Type viewModel)
    {
        var registry = new NavigationRegistry()
        {
            Name = view.Name,
            ViewType = view,
            ViewModelType = viewModel
        };

        sc.AddSingleton(registry);

        sc.AddScoped(view);
        sc.AddScoped(viewModel);
    }

    public static void AddTransientViewScopedViewModelForNavigation(this IServiceCollection sc, Type view, Type viewModel)
    {
        var registry = new NavigationRegistry()
        {
            Name = view.Name,
            ViewType = view,
            ViewModelType = viewModel
        };

        sc.AddSingleton(registry);

        sc.AddTransient(view);
        sc.AddScoped(viewModel);
    }
}
