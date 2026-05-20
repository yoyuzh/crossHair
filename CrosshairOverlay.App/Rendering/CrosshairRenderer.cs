using System.Windows;
using System.Windows.Media;
using CrosshairOverlay.App.Models;

namespace CrosshairOverlay.App.Rendering;

public sealed class CrosshairRenderer : FrameworkElement
{
    public static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register(
            nameof(Settings),
            typeof(CrosshairSettings),
            typeof(CrosshairRenderer),
            new FrameworkPropertyMetadata(CrosshairSettings.CreateDefault(), FrameworkPropertyMetadataOptions.AffectsRender));

    public CrosshairSettings Settings
    {
        get => (CrosshairSettings)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (!Settings.Enabled)
        {
            return;
        }

        if (!TryParseColor(Settings.Color, out var color))
        {
            color = Colors.Lime;
        }
        color.A = (byte)Math.Round(255 * Settings.Opacity);
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        var pen = new Pen(brush, Settings.Thickness)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round
        };
        pen.Freeze();

        var geometry = CrosshairGeometryBuilder.Build(Settings, ActualWidth, ActualHeight);

        foreach (var line in geometry.Lines)
        {
            drawingContext.DrawLine(pen, new Point(line.X1, line.Y1), new Point(line.X2, line.Y2));
        }

        foreach (var circle in geometry.Circles)
        {
            if (circle.Filled)
            {
                drawingContext.DrawEllipse(brush, null, new Point(circle.CenterX, circle.CenterY), circle.Radius, circle.Radius);
            }
            else
            {
                drawingContext.DrawEllipse(null, pen, new Point(circle.CenterX, circle.CenterY), circle.Radius, circle.Radius);
            }
        }
    }

    private static bool TryParseColor(string value, out Color color)
    {
        try
        {
            var parsed = ColorConverter.ConvertFromString(value);
            if (parsed is Color parsedColor)
            {
                color = parsedColor;
                return true;
            }
        }
        catch (FormatException)
        {
            color = Colors.Lime;
            return false;
        }
    }
}
