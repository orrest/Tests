using CommunityToolkit.Mvvm.ComponentModel;
using Orrest.Controls.Themes;
using Tests.Controls.Constants;

namespace Tests.Controls.Themes;

public partial class ThemeSwitcherViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsLightSelected { get; set; }

    [ObservableProperty]
    public partial bool IsDarkSelected { get; set; }

    [ObservableProperty]
    public partial bool IsSystemSelected { get; set; }

    [ObservableProperty]
    public partial string CurrentThemeText { get; set; }

    public ThemeSwitcherViewModel()
    {
        // Reflect the current theme mode on startup
        var current = ThemeManager.CurrentMode;
        if (current == ThemeMode.Light)
        {
            IsLightSelected = true;
            CurrentThemeText = "Light";
        }
        else if (current == ThemeMode.Dark)
        {
            IsDarkSelected = true;
            CurrentThemeText = "Dark";
        }
        else
        {
            IsSystemSelected = true;
            CurrentThemeText = "System";
        }
    }

    partial void OnIsLightSelectedChanged(bool value)
    {
        if (!value)
        {
            return;
        }

        ApplyTheme(ThemeMode.Light, "Light");
    }

    partial void OnIsDarkSelectedChanged(bool value)
    {
        if (!value)
        {
            return;
        }

        ApplyTheme(ThemeMode.Dark, "Dark");
    }

    partial void OnIsSystemSelectedChanged(bool value)
    {
        if (!value)
        {
            return;
        }

        ApplyTheme(ThemeMode.System, "System");
    }

    private void ApplyTheme(ThemeMode mode, string name)
    {
        // Uncheck the other radio buttons without re-entering
        if (mode != ThemeMode.Light)
        {
            IsLightSelected = false;
        }

        if (mode != ThemeMode.Dark)
        {
            IsDarkSelected = false;
        }

        if (mode != ThemeMode.System)
        {
            IsSystemSelected = false;
        }

        // Apply the theme via ThemeManager
        ThemeManager.SwitchTheme(mode);
        CurrentThemeText = name;

        // Notify via toast
        Messenger.Send($"Theme switched to {name}", Channels.TOAST);
    }
}