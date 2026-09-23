using MudBlazor;

namespace Arahk.Neighbor.Web.Theme;

public static class NeighborTheme
{
    public static MudTheme Create() => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#2F9E7A",
            Secondary = "#E8A25B",
            Background = "#F7F3EB",
            Surface = "#FFFFFF",
            AppbarBackground = "#2F9E7A",
            AppbarText = "#FFFFFF",
            TextPrimary = "#1A1A1A",
            TextSecondary = "#5C5C5C",
            Error = "#C23B3B",
            DrawerBackground = "#FFFFFF",
            DrawerText = "#1A1A1A",
            LinesDefault = "#E2DDD4",
            Divider = "#E2DDD4",
            ActionDisabledBackground = "#EDE9E1",
            ActionDisabled = "#A8A39A",
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["Noto Sans Thai", "Sarabun", "system-ui", "Segoe UI", "Roboto", "Helvetica Neue", "Arial", "sans-serif"],
                FontSize = "0.9375rem",
            },
            H5 = new H5Typography { FontWeight = "600", FontSize = "1.25rem" },
            Button = new ButtonTypography { FontWeight = "600", FontSize = "1rem", TextTransform = "none" },
        },
        LayoutProperties = new LayoutProperties { DefaultBorderRadius = "12px" }
    };
}
