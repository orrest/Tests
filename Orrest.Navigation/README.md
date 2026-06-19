# Orrest.Navigation

A lightweight .NET library for registering View/ViewModel pairs in your dependency injection container with explicit lifetime control. Designed for WPF navigation.

[![NuGet](https://img.shields.io/nuget/v/Orrest.Navigation)](https://www.nuget.org/packages/Orrest.Navigation)
[![Target framework](https://img.shields.io/badge/dynamic/xml?label=target&query=%2FProject%2FPropertyGroup%2FTargetFramework&url=https%3A%2F%2Fraw.githubusercontent.com%2Forrest%2FOrrest.Navigation%2Fmain%2FOrrest.Navigation.csproj)](https://github.com/orrest/Orrest.Navigation)

---

## Installation

```shell
dotnet add package Orrest.Navigation
```

Or via the NuGet Package Manager:

```
Install-Package Orrest.Navigation
```

## Quick start

```csharp
using Microsoft.Extensions.DependencyInjection;
using Orrest.Navigation;

// In your service collection setup:
services.AddSingletonForNavigation(typeof(MainView), typeof(MainViewModel));
services.AddTransientForNavigation(typeof(DetailView), typeof(DetailViewModel));
services.AddTransientViewSingletonViewModelForNavigation(typeof(EditorView), typeof(EditorViewModel));
```

Each call registers:
1. A singleton `NavigationRegistry` (used for runtime navigation lookups).
2. The `View` and `ViewModel` types with your chosen lifetime.

Later, resolve all registered navigation targets:

```csharp
var registries = serviceProvider.GetServices<NavigationRegistry>();
```

Or use `NavigationItem` to build a menu or navigation list:

```csharp
var items = registries.Select(r => new NavigationItem(
    r.Name.Replace("View", ""),   // display title
    r.Name                         // view name for lookup
));
```

## API

### Extension methods on `IServiceCollection`

| Method | View lifetime | ViewModel lifetime | Use case |
|---|---|---|---|
| `AddSingletonForNavigation(Type view, Type viewModel)` | Singleton | Singleton | Shared views like main window or shell |
| `AddTransientForNavigation(Type view, Type viewModel)` | Transient | Transient | Ephemeral dialogs or pages |
| `AddTransientViewSingletonViewModelForNavigation(Type view, Type viewModel)` | Transient | Singleton | Views that come and go but share one VM |
| `AddScopedForNavigation(Type view, Type viewModel)` | Scoped | Scoped | Per-scope (e.g. per request or per session) pages and view models |
| `AddTransientViewScopedViewModelForNavigation(Type view, Type viewModel)` | Transient | Scoped | Views created each time but share a scoped ViewModel |

### Types

- **`NavigationRegistry`** — Entry stored per registration:
  - `Name` (`string`) — the view's type name (e.g. `"MainView"`)
  - `ViewType` (`Type`) — the registered view type
  - `ViewModelType` (`Type`) — the registered view model type

- **`NavigationItem`** — A simple record for UI binding:
  - `Title` (`string`) — display name
  - `ViewName` (`string`) — view identifier for navigation resolution

## Dependencies

- [Microsoft.Extensions.DependencyInjection.Abstractions](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection.Abstractions/) (≥ 10.0.5)
- .NET 10.0+

## Compatibility

Targets `net10.0` and works with the generic host / `Microsoft.Extensions.DependencyInjection` DI container. Compatible with WPF, Avalonia, MAUI, and any other UI framework built on the standard Microsoft DI abstractions.

## License

This project is licensed under the MIT License.
