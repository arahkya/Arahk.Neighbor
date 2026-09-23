using MudBlazor;

namespace Arahk.Neighbor.Web.Theme;

/// <summary>
/// MudBlazor theme aligned with Neighbor Design System colors / type / radii.
/// Spacing and control heights use Mud defaults + utilities (<c>pa-</c>/<c>Size</c>).
/// AppBar height: Mud default 64px (prefer over Dense/56 per DS SoT).
/// </summary>
public static class NeighborTheme
{
    public static MudTheme Create() => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = NeighborDesignTokens.ColorPrimary,
            Secondary = NeighborDesignTokens.ColorAccent,
            Background = NeighborDesignTokens.ColorBg,
            Surface = NeighborDesignTokens.ColorSurface,
            AppbarBackground = NeighborDesignTokens.ColorSurface,
            AppbarText = NeighborDesignTokens.ColorTextPrimary,
            TextPrimary = NeighborDesignTokens.ColorTextPrimary,
            TextSecondary = NeighborDesignTokens.ColorTextSecondary,
            Error = NeighborDesignTokens.ColorError,
            DrawerBackground = NeighborDesignTokens.ColorSurface,
            DrawerText = NeighborDesignTokens.ColorTextPrimary,
            LinesDefault = NeighborDesignTokens.ColorBorder,
            Divider = NeighborDesignTokens.ColorBorder,
            ActionDisabledBackground = NeighborDesignTokens.ColorDisabledBg,
            ActionDisabled = NeighborDesignTokens.ColorDisabledText,
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = NeighborDesignTokens.FontFamily,
                FontSize = NeighborDesignTokens.FontSizeBody,
                FontWeight = NeighborDesignTokens.FontWeightRegular,
                LineHeight = NeighborDesignTokens.LineHeightBody,
            },
            H5 = new H5Typography
            {
                FontSize = NeighborDesignTokens.FontSizeTitle,
                FontWeight = NeighborDesignTokens.FontWeightSemibold,
                LineHeight = NeighborDesignTokens.LineHeightTitle,
            },
            H6 = new H6Typography
            {
                FontSize = NeighborDesignTokens.FontSizeWordmark,
                FontWeight = NeighborDesignTokens.FontWeightBold,
                LineHeight = NeighborDesignTokens.LineHeightWordmark,
            },
            Body1 = new Body1Typography
            {
                FontSize = NeighborDesignTokens.FontSizeBody,
                FontWeight = NeighborDesignTokens.FontWeightRegular,
                LineHeight = NeighborDesignTokens.LineHeightBody,
            },
            Body2 = new Body2Typography
            {
                FontSize = NeighborDesignTokens.FontSizeLink,
                FontWeight = NeighborDesignTokens.FontWeightMedium,
                LineHeight = NeighborDesignTokens.LineHeightLabel,
            },
            Caption = new CaptionTypography
            {
                FontSize = NeighborDesignTokens.FontSizeHelper,
                FontWeight = NeighborDesignTokens.FontWeightRegular,
                LineHeight = NeighborDesignTokens.LineHeightHelper,
            },
            Button = new ButtonTypography
            {
                FontSize = NeighborDesignTokens.FontSizeButton,
                FontWeight = NeighborDesignTokens.FontWeightSemibold,
                TextTransform = "none",
                LineHeight = NeighborDesignTokens.LineHeightButton,
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontSize = NeighborDesignTokens.FontSizeLabel,
                FontWeight = NeighborDesignTokens.FontWeightSemibold,
                LineHeight = NeighborDesignTokens.LineHeightLabel,
            },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = NeighborDesignTokens.RadiusButton,
            // AppbarHeight: leave Mud default 64px (DS SoT; not Dense/56).
            // Drawer wider than Mud 240px for Thai nav labels (product lock).
            DrawerWidthLeft = NeighborDesignTokens.DrawerWidth,
            DrawerWidthRight = NeighborDesignTokens.DrawerWidth,
        },
    };
}
