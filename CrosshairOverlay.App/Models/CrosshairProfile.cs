namespace CrosshairOverlay.App.Models;

public sealed class CrosshairProfile
{
    public string Name { get; set; } = "Profile";
    public CrosshairStyle Style { get; set; } = CrosshairStyle.CrossDot;
    public double Size { get; set; } = 16;
    public double Thickness { get; set; } = 2;
    public double Gap { get; set; } = 4;
    public double Opacity { get; set; } = 0.9;
    public string Color { get; set; } = "#00FF00";
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }

    public static CrosshairProfile FromSettings(CrosshairSettings settings, string name)
    {
        return new CrosshairProfile
        {
            Name = name,
            Style = settings.Style,
            Size = settings.Size,
            Thickness = settings.Thickness,
            Gap = settings.Gap,
            Opacity = settings.Opacity,
            Color = settings.Color,
            OffsetX = settings.OffsetX,
            OffsetY = settings.OffsetY
        };
    }

    public void ApplyTo(CrosshairSettings settings)
    {
        settings.Style = Style;
        settings.Size = Size;
        settings.Thickness = Thickness;
        settings.Gap = Gap;
        settings.Opacity = Opacity;
        settings.Color = Color;
        settings.OffsetX = OffsetX;
        settings.OffsetY = OffsetY;
    }
}
