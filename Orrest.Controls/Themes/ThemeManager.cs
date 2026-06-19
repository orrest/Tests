using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;

// ReSharper disable once CheckNamespace
namespace Orrest.Controls.Themes;

/// <summary>
///     管理应用主题切换、系统主题监听和主题颜色读取。
/// </summary>
public static class ThemeManager
{
    private const string LightThemeUri = "pack://application:,,,/Orrest.Controls;component/Themes/Light.xaml";
    private const string DarkThemeUri = "pack://application:,,,/Orrest.Controls;component/Themes/Dark.xaml";
    private const string ControlsStyleUri = "pack://application:,,,/Orrest.Controls;component/Themes/Generic.xaml";

    private static ResourceDictionary? _currentThemeDict;
    private static ResourceDictionary? _controlsStyleDict;
    private static bool _initialized;

    /// <summary>
    ///     当前主题模式。
    /// </summary>
    public static ThemeMode CurrentMode { get; private set; } = ThemeMode.System;

    /// <summary>
    ///     当前主题的全部颜色值。
    /// </summary>
    public static ThemeColors ThemeColors { get; private set; } = new();

    /// <summary>
    ///     主题切换后触发，传递当前 <see cref="ThemeColors" />。
    /// </summary>
    public static event EventHandler<ThemeColors>? ThemeChanged;

    /// <summary>
    ///     初始化主题管理器。应在 Application 启动时调用一次。
    /// </summary>
    /// <param name="initialMode">初始主题模式，默认为 System。</param>
    public static void Initialize(ThemeMode initialMode = ThemeMode.System)
    {
        if (_initialized) return;
        _initialized = true;

        CurrentMode = initialMode;

        // 加载控件样式字典（Generic.xaml），只需加载一次，不随主题切换替换
        if (Application.Current is { } app)
        {
            var controlsDict = new ResourceDictionary
            {
                Source = new Uri(ControlsStyleUri, UriKind.Absolute)
            };
            app.Resources.MergedDictionaries.Insert(0, controlsDict);
            _controlsStyleDict = controlsDict;
        }

        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        ApplyTheme(ResolveMode(initialMode));
    }

    /// <summary>
    ///     切换到指定主题模式。
    /// </summary>
    public static void SwitchTheme(ThemeMode mode)
    {
        CurrentMode = mode;
        ApplyTheme(ResolveMode(mode));
    }

    /// <summary>
    ///     根据当前模式解析实际要使用的主题（System 模式下跟随系统）。
    /// </summary>
    private static bool ResolveMode(ThemeMode mode)
    {
        return mode switch
        {
            ThemeMode.Light => false,
            ThemeMode.Dark => true,
            _ => GetSystemIsDark()
        };
    }

    /// <summary>
    ///     应用浅色或深色主题字典。
    /// </summary>
    private static void ApplyTheme(bool isDark)
    {
        if (Application.Current is not { } app) return;

        // 确保在 UI 线程上操作
        if (!app.Dispatcher.CheckAccess())
        {
            app.Dispatcher.Invoke(() => ApplyTheme(isDark));
            return;
        }

        var uri = new Uri(isDark ? DarkThemeUri : LightThemeUri, UriKind.Absolute);
        var newDict = new ResourceDictionary { Source = uri };

        if (_currentThemeDict is not null)
        {
            var index = app.Resources.MergedDictionaries.IndexOf(_currentThemeDict);
            if (index >= 0)
                app.Resources.MergedDictionaries[index] = newDict;
            else
                app.Resources.MergedDictionaries.Add(newDict);
        }
        else
        {
            app.Resources.MergedDictionaries.Add(newDict);
        }

        _currentThemeDict = newDict;
        ThemeColors = ReadThemeColors();
        ThemeChanged?.Invoke(null, ThemeColors);
    }

    /// <summary>
    ///     从 Application 资源中读取所有主题颜色。
    /// </summary>
    private static ThemeColors ReadThemeColors()
    {
        return new ThemeColors
        {
            // Basic Colors
            ForegroundColor = TryGetColor("ForegroundColor"),
            ForegroundLeadColor = TryGetColor("ForegroundLeadColor"),
            BackgroundColor = TryGetColor("BackgroundColor"),
            MutedColor = TryGetColor("MutedColor"),
            BorderColor = TryGetColor("BorderColor"),
            BorderColor60 = TryGetColor("BorderColor60"),
            BorderColor30 = TryGetColor("BorderColor30"),
            OutlineColor = TryGetColor("OutlineColor"),
            GhostColor = TryGetColor("GhostColor"),
            GhostHoverColor = TryGetColor("GhostHoverColor"),
            GhostHoverColor50 = TryGetColor("GhostHoverColor50"),

            // Theme Colors
            PrimaryColor = TryGetColor("PrimaryColor"),
            PrimaryColor75 = TryGetColor("PrimaryColor75"),
            PrimaryColor50 = TryGetColor("PrimaryColor50"),
            PrimaryColor10 = TryGetColor("PrimaryColor10"),
            PrimaryForegroundColor = TryGetColor("PrimaryForegroundColor"),
            SecondaryColor = TryGetColor("SecondaryColor"),
            SecondaryColor75 = TryGetColor("SecondaryColor75"),
            SecondaryColor50 = TryGetColor("SecondaryColor50"),
            SecondaryForegroundColor = TryGetColor("SecondaryForegroundColor"),
            DestructiveColor = TryGetColor("DestructiveColor"),
            DestructiveColor75 = TryGetColor("DestructiveColor75"),
            DestructiveColor50 = TryGetColor("DestructiveColor50"),
            DestructiveColor10 = TryGetColor("DestructiveColor10"),
            DestructiveForegroundColor = TryGetColor("DestructiveForegroundColor"),

            // Notification Colors
            InfoColor = TryGetColor("InfoColor"),
            InfoColor60 = TryGetColor("InfoColor60"),
            InfoColor20 = TryGetColor("InfoColor20"),
            InfoColor10 = TryGetColor("InfoColor10"),
            InfoColor5 = TryGetColor("InfoColor5"),
            SuccessColor = TryGetColor("SuccessColor"),
            SuccessColor60 = TryGetColor("SuccessColor60"),
            SuccessColor20 = TryGetColor("SuccessColor20"),
            SuccessColor10 = TryGetColor("SuccessColor10"),
            SuccessColor5 = TryGetColor("SuccessColor5"),
            WarningColor = TryGetColor("WarningColor"),
            WarningColor60 = TryGetColor("WarningColor60"),
            WarningColor20 = TryGetColor("WarningColor20"),
            WarningColor10 = TryGetColor("WarningColor10"),
            WarningColor5 = TryGetColor("WarningColor5"),
            ErrorColor = TryGetColor("ErrorColor"),
            ErrorColor60 = TryGetColor("ErrorColor60"),
            ErrorColor20 = TryGetColor("ErrorColor20"),
            ErrorColor10 = TryGetColor("ErrorColor10"),
            ErrorColor5 = TryGetColor("ErrorColor5"),

            // Specific Control Colors
            BusyAreaOverlayColor = TryGetColor("BusyAreaOverlayColor"),
            CardBackgroundColor = TryGetColor("CardBackgroundColor"),
            DialogOverlayColor = TryGetColor("DialogOverlayColor"),
            DialogBackgroundColor = TryGetColor("DialogBackgroundColor"),
            TitleBarBackgroundColor = TryGetColor("TitleBarBackgroundColor"),
            WindowBackgroundColor = TryGetColor("WindowBackgroundColor"),
            WindowButtonHoverColor = TryGetColor("WindowButtonHoverColor"),
            SidebarBackgroundColor = TryGetColor("SidebarBackgroundColor"),
            SwitchBackgroundColor = TryGetColor("SwitchBackgroundColor"),
            SwitchForegroundColor = TryGetColor("SwitchForegroundColor"),
            TabItemSelectedColor = TryGetColor("TabItemSelectedColor"),
            TabItemsBackgroundColor = TryGetColor("TabItemsBackgroundColor")
        };
    }

    /// <summary>
    ///     从 Application 资源中查找颜色，找不到时返回默认值。
    /// </summary>
    private static Color TryGetColor(string resourceKey)
    {
        var resource = Application.Current.TryFindResource(resourceKey);

        if (resource is SolidColorBrush brush)
            return brush.Color;

        if (resource is Color color)
            return color;

        return default;
    }

    /// <summary>
    ///     系统主题偏好变化时重新应用主题。
    /// </summary>
    private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (CurrentMode == ThemeMode.System)
        {
            Application.Current.Dispatcher.Invoke(() => ApplyTheme(GetSystemIsDark()));
        }
    }

    /// <summary>
    ///     检测 Windows 系统是否为深色模式。
    /// </summary>
    private static bool GetSystemIsDark()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            if (key?.GetValue("AppsUseLightTheme") is int value)
                return value == 0;
        }
        catch
        {
            // 忽略注册表读取失败
        }

        return false;
    }
}
