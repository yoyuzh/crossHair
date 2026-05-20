using System.Windows;
using System.Windows.Media;
using CrosshairOverlay.App.Models;
using MediaColor = System.Windows.Media.Color;
using MediaColorConverter = System.Windows.Media.ColorConverter;
using MediaPen = System.Windows.Media.Pen;
using WpfPoint = System.Windows.Point;

namespace CrosshairOverlay.App.Rendering;

public sealed class CrosshairRenderer : FrameworkElement
{
    public static readonly DependencyProperty SettingsProperty =
        DependencyProperty.Register(
            nameof(Settings),
            typeof(CrosshairSettings),
            typeof(CrosshairRenderer),
            new FrameworkPropertyMetadata(CrosshairSettings.CreateDefault(), FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty IgnoreEnabledProperty =
        DependencyProperty.Register(
            nameof(IgnoreEnabled),
            typeof(bool),
            typeof(CrosshairRenderer),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

    public CrosshairSettings Settings
    {
        get => (CrosshairSettings)GetValue(SettingsProperty);
        set => SetValue(SettingsProperty, value);
    }

    public bool IgnoreEnabled
    {
        get => (bool)GetValue(IgnoreEnabledProperty);
        set => SetValue(IgnoreEnabledProperty, value);
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);

        if (!Settings.Enabled && !IgnoreEnabled)
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
        var pen = new MediaPen(brush, Settings.Thickness)
        {
            StartLineCap = PenLineCap.Round,
            EndLineCap = PenLineCap.Round
        };
        pen.Freeze();

        var geometry = CrosshairGeometryBuilder.Build(Settings, ActualWidth, ActualHeight);

        foreach (var line in geometry.Lines)
        {
            drawingContext.DrawLine(pen, new WpfPoint(line.X1, line.Y1), new WpfPoint(line.X2, line.Y2));
        }

        foreach (var circle in geometry.Circles)
        {
            if (circle.Filled)
            {
                drawingContext.DrawEllipse(brush, null, new WpfPoint(circle.CenterX, circle.CenterY), circle.Radius, circle.Radius);
            }
            else
            {
                drawingContext.DrawEllipse(null, pen, new WpfPoint(circle.CenterX, circle.CenterY), circle.Radius, circle.Radius);
            }
        }
    }

    private static bool TryParseColor(string value, out MediaColor color)
    {
        try
        {
            var parsed = MediaColorConverter.ConvertFromString(value);
            if (parsed is MediaColor parsedColor)
            {
                color = parsedColor;
                return true;
            }
            color = Colors.Lime;
            return false;
        }
        catch (FormatException)
        {
            color = Colors.Lime;
            return false;
        }
    }
}
