namespace Arahk.Neighbor.Web.Theme;

/// <summary>
/// C# mirror of Neighbor Design System color / type / radius tokens
/// (<c>wwwroot/nb-tokens.css</c> / <c>--nb-*</c>).
/// Spacing and control heights follow MudBlazor 9 (utilities + <see cref="MudBlazor.Size"/>);
/// do not reintroduce Size*/Space* constants that fight Mud defaults.
/// </summary>
public static class NeighborDesignTokens
{
    // Colors — palette locked: #2F9E7A / #F7F3EB / #E8A25B
    public const string ColorPrimary = "#2F9E7A";
    public const string ColorPrimaryHover = "#278A6A";
    public const string ColorPrimaryActive = "#217359";
    public const string ColorPrimaryMuted = "#E6F5EF";
    public const string ColorBg = "#F7F3EB";
    public const string ColorSurface = "#FFFFFF";
    public const string ColorSurfaceSubtle = "#FBF9F5";
    public const string ColorAccent = "#E8A25B";
    public const string ColorAccentSoft = "#FDF3E8";
    public const string ColorTextPrimary = "#1A1A1A";
    public const string ColorTextSecondary = "#5C5C5C";
    public const string ColorTextMuted = "#8A8A8A";
    public const string ColorTextOnPrimary = "#FFFFFF";
    public const string ColorTextPlaceholder = "#9A9A9A";
    public const string ColorBorder = "#E2DDD4";
    public const string ColorBorderStrong = "#C8C2B6";
    public const string ColorError = "#C23B3B";
    public const string ColorErrorBg = "#FDF0F0";
    public const string ColorDisabledBg = "#EDE9E1";
    public const string ColorDisabledText = "#A8A39A";

    // Typography
    public static readonly string[] FontFamily =
    [
        "Noto Sans Thai",
        "Sarabun",
        "system-ui",
        "Segoe UI",
        "Roboto",
        "Helvetica Neue",
        "Arial",
        "sans-serif",
    ];

    public const string FontSizeBody = "0.9375rem";   // 15px — --nb-font-size-body
    public const string FontSizeTitle = "1.25rem";    // 20px — --nb-font-size-title
    public const string FontSizeWordmark = "1.5rem";  // 24px — --nb-font-size-wordmark
    public const string FontSizeLabel = "0.8125rem";  // 13px — --nb-font-size-label
    public const string FontSizeHelper = "0.75rem";   // 12px — --nb-font-size-helper
    public const string FontSizeButton = "1rem";      // 16px — --nb-font-size-button
    public const string FontSizeLink = "0.8125rem";   // 13px — --nb-font-link

    public const string FontWeightRegular = "400";
    public const string FontWeightMedium = "500";
    public const string FontWeightSemibold = "600";
    public const string FontWeightBold = "700";

    public const string LineHeightBody = "1.5";
    public const string LineHeightTitle = "1.3";
    public const string LineHeightWordmark = "1.2";
    public const string LineHeightLabel = "1.4";
    public const string LineHeightHelper = "1.45";
    public const string LineHeightButton = "1.25";

    // Radii (string form for MudBlazor LayoutProperties)
    public const string RadiusButton = "12px"; // --nb-radius-button
    public const string RadiusInput = "10px";  // --nb-radius-input
    public const string RadiusCard = "16px";   // --nb-radius-card

    public const string BorderWidth = "1.5px";

    /// <summary>
    /// Drawer width above Mud default 240px — Thai nav labels need the extra room.
    /// Wired via <see cref="NeighborTheme"/> LayoutProperties.DrawerWidthLeft/Right.
    /// </summary>
    public const string DrawerWidth = "272px";
}
