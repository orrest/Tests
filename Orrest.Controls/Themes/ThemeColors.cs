using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace Orrest.Controls.Themes;

/// <summary>
///     存储应用中使用的所有主题颜色值。
/// </summary>
public class ThemeColors
{
    // ── 基础色 ──────────────────────────────────────────────
    public Color ForegroundColor { get; init; }
    public Color ForegroundLeadColor { get; init; }
    public Color BackgroundColor { get; init; }
    public Color MutedColor { get; init; }
    public Color BorderColor { get; init; }
    public Color BorderColor60 { get; init; }
    public Color BorderColor30 { get; init; }
    public Color OutlineColor { get; init; }
    public Color GhostColor { get; init; }
    public Color GhostHoverColor { get; init; }
    public Color GhostHoverColor50 { get; init; }
    public Color SelectionColor { get; init; }

    // ── 主题色 ──────────────────────────────────────────────
    public Color PrimaryColor { get; init; }
    public Color PrimaryColor75 { get; init; }
    public Color PrimaryColor50 { get; init; }
    public Color PrimaryColor10 { get; init; }
    public Color PrimaryForegroundColor { get; init; }
    public Color SecondaryColor { get; init; }
    public Color SecondaryColor75 { get; init; }
    public Color SecondaryColor50 { get; init; }
    public Color SecondaryForegroundColor { get; init; }
    public Color DestructiveColor { get; init; }
    public Color DestructiveColor75 { get; init; }
    public Color DestructiveColor50 { get; init; }
    public Color DestructiveColor10 { get; init; }
    public Color DestructiveForegroundColor { get; init; }

    // ── 通知色 ──────────────────────────────────────────────
    public Color InfoColor { get; init; }
    public Color InfoColor60 { get; init; }
    public Color InfoColor20 { get; init; }
    public Color InfoColor10 { get; init; }
    public Color InfoColor5 { get; init; }
    public Color SuccessColor { get; init; }
    public Color SuccessColor60 { get; init; }
    public Color SuccessColor20 { get; init; }
    public Color SuccessColor10 { get; init; }
    public Color SuccessColor5 { get; init; }
    public Color WarningColor { get; init; }
    public Color WarningColor60 { get; init; }
    public Color WarningColor20 { get; init; }
    public Color WarningColor10 { get; init; }
    public Color WarningColor5 { get; init; }
    public Color ErrorColor { get; init; }
    public Color ErrorColor60 { get; init; }
    public Color ErrorColor20 { get; init; }
    public Color ErrorColor10 { get; init; }
    public Color ErrorColor5 { get; init; }

    // ── 特定控件色 ──────────────────────────────────────────
    public Color BusyAreaOverlayColor { get; init; }
    public Color CardBackgroundColor { get; init; }
    public Color DialogOverlayColor { get; init; }
    public Color DialogBackgroundColor { get; init; }
    public Color TitleBarBackgroundColor { get; init; }
    public Color WindowBackgroundColor { get; init; }
    public Color WindowButtonHoverColor { get; init; }
    public Color SidebarBackgroundColor { get; init; }
    public Color SwitchBackgroundColor { get; init; }
    public Color SwitchForegroundColor { get; init; }
    public Color TabItemSelectedColor { get; init; }
    public Color TabItemsBackgroundColor { get; init; }
}
