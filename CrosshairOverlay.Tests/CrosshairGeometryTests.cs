using CrosshairOverlay.App.Models;
using CrosshairOverlay.App.Rendering;
using Xunit;

namespace CrosshairOverlay.Tests;

public sealed class CrosshairGeometryTests
{
    [Fact]
    public void CrossDotCreatesFourLinesAroundCenterAndADot()
    {
        var settings = CrosshairSettings.CreateDefault();
        settings.Style = CrosshairStyle.CrossDot;
        settings.Size = 10;
        settings.Gap = 4;
        settings.Thickness = 2;

        var geometry = CrosshairGeometryBuilder.Build(settings, 100, 80);

        Assert.Equal(4, geometry.Lines.Count);
        Assert.Contains(new CrosshairLine(36, 40, 46, 40, 2), geometry.Lines);
        Assert.Contains(new CrosshairLine(54, 40, 64, 40, 2), geometry.Lines);
        Assert.Contains(new CrosshairLine(50, 26, 50, 36, 2), geometry.Lines);
        Assert.Contains(new CrosshairLine(50, 44, 50, 54, 2), geometry.Lines);
        var dot = Assert.Single(geometry.Circles);
        Assert.True(dot.Filled);
        Assert.Equal(50, dot.CenterX);
        Assert.Equal(40, dot.CenterY);
        Assert.Equal(2, dot.Radius);
    }

    [Fact]
    public void OffsetMovesGeometryCenter()
    {
        var settings = CrosshairSettings.CreateDefault();
        settings.Style = CrosshairStyle.Dot;
        settings.OffsetX = 7;
        settings.OffsetY = -5;

        var geometry = CrosshairGeometryBuilder.Build(settings, 100, 80);

        var dot = Assert.Single(geometry.Circles);
        Assert.Equal(57, dot.CenterX);
        Assert.Equal(35, dot.CenterY);
    }

    [Fact]
    public void CircleStyleCreatesOutlinedCircle()
    {
        var settings = CrosshairSettings.CreateDefault();
        settings.Style = CrosshairStyle.Circle;
        settings.Size = 18;
        settings.Thickness = 3;

        var geometry = CrosshairGeometryBuilder.Build(settings, 100, 80);

        Assert.Empty(geometry.Lines);
        var circle = Assert.Single(geometry.Circles);
        Assert.False(circle.Filled);
        Assert.Equal(18, circle.Radius);
        Assert.Equal(3, circle.Thickness);
    }
}
