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
                FontSize = "0.9375rem", // 15px body
                FontWeight = "400",
                LineHeight = "1.5",
            },
            H5 = new H5Typography
            {
                FontSize = "1.25rem", // 20px title
                FontWeight = "600",
                LineHeight = "1.3",
            },
            H6 = new H6Typography
            {
                FontSize = "1.5rem", // 24px wordmark contexts
                FontWeight = "700",
                LineHeight = "1.2",
            },
            Body1 = new Body1Typography
            {
                FontSize = "0.9375rem", // 15px
                FontWeight = "400",
                LineHeight = "1.5",
            },
            Body2 = new Body2Typography
            {
                FontSize = "0.8125rem", // 13px link / secondary
                FontWeight = "500",
                LineHeight = "1.4",
            },
            Caption = new CaptionTypography
            {
                FontSize = "0.75rem", // 12px helper
                FontWeight = "400",
                LineHeight = "1.45",
            },
            Button = new ButtonTypography
            {
                FontSize = "1rem", // 16px
                FontWeight = "600",
                TextTransform = "none",
                LineHeight = "1.25",
            },
            Subtitle2 = new Subtitle2Typography
            {
                FontSize = "0.8125rem", // 13px label
                FontWeight = "600",
                LineHeight = "1.4",
            },
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "12px",
        },
    };
}
