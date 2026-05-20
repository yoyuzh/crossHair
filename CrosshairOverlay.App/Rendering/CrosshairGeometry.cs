using CrosshairOverlay.App.Models;

namespace CrosshairOverlay.App.Rendering;

public sealed record CrosshairLine(double X1, double Y1, double X2, double Y2, double Thickness);

public sealed record CrosshairCircle(double CenterX, double CenterY, double Radius, double Thickness, bool Filled);

public sealed record CrosshairGeometry(
    IReadOnlyList<CrosshairLine> Lines,
    IReadOnlyList<CrosshairCircle> Circles);

public static class CrosshairGeometryBuilder
{
    public static CrosshairGeometry Build(CrosshairSettings settings, double width, double height)
    {
        var centerX = width / 2d + settings.OffsetX;
        var centerY = height / 2d + settings.OffsetY;
        var lines = new List<CrosshairLine>();
        var circles = new List<CrosshairCircle>();

        if (settings.Style is CrosshairStyle.Cross or CrosshairStyle.CrossDot)
        {
            lines.Add(new CrosshairLine(centerX - settings.Gap - settings.Size, centerY, centerX - settings.Gap, centerY, settings.Thickness));
            lines.Add(new CrosshairLine(centerX + settings.Gap, centerY, centerX + settings.Gap + settings.Size, centerY, settings.Thickness));
            lines.Add(new CrosshairLine(centerX, centerY - settings.Gap - settings.Size, centerX, centerY - settings.Gap, settings.Thickness));
            lines.Add(new CrosshairLine(centerX, centerY + settings.Gap, centerX, centerY + settings.Gap + settings.Size, settings.Thickness));
        }

        if (settings.Style is CrosshairStyle.Dot or CrosshairStyle.CrossDot)
        {
            circles.Add(new CrosshairCircle(centerX, centerY, Math.Max(settings.Thickness, 1), settings.Thickness, true));
        }

        if (settings.Style is CrosshairStyle.Circle)
        {
            circles.Add(new CrosshairCircle(centerX, centerY, settings.Size, settings.Thickness, false));
        }

        return new CrosshairGeometry(lines, circles);
    }
}
