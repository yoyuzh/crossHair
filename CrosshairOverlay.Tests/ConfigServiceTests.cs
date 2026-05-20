using CrosshairOverlay.App.Config;
using CrosshairOverlay.App.Models;
using Xunit;

namespace CrosshairOverlay.Tests;

public sealed class ConfigServiceTests
{
    [Fact]
    public void LoadReturnsDefaultsWhenConfigIsMissing()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "config.json");
        var service = new ConfigService(path);

        var settings = service.Load();

        Assert.True(settings.Enabled);
        Assert.Equal(CrosshairStyle.CrossDot, settings.Style);
        Assert.Equal("#00FF00", settings.Color);
        Assert.Equal("F8", settings.ToggleHotkey);
    }

    [Fact]
    public void SaveAndLoadRoundTripsSettings()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        var path = Path.Combine(directory, "config.json");
        var service = new ConfigService(path);
        var original = CrosshairSettings.CreateDefault();
        original.Style = CrosshairStyle.Circle;
        original.Size = 24;
        original.Thickness = 3;
        original.Gap = 8;
        original.Opacity = 0.7;
        original.Color = "#FF0000";
        original.OffsetX = 5;
        original.OffsetY = -3;
        original.ComplianceNoticeAccepted = true;

        service.Save(original);
        var loaded = service.Load();

        Assert.Equal(CrosshairStyle.Circle, loaded.Style);
        Assert.Equal(24, loaded.Size);
        Assert.Equal(3, loaded.Thickness);
        Assert.Equal(8, loaded.Gap);
        Assert.Equal(0.7, loaded.Opacity);
        Assert.Equal("#FF0000", loaded.Color);
        Assert.Equal(5, loaded.OffsetX);
        Assert.Equal(-3, loaded.OffsetY);
        Assert.True(loaded.ComplianceNoticeAccepted);
    }

    [Fact]
    public void LoadReturnsDefaultsWhenJsonIsInvalid()
    {
        var directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "config.json");
        File.WriteAllText(path, "{ invalid json");
        var service = new ConfigService(path);

        var settings = service.Load();

        Assert.Equal(CrosshairStyle.CrossDot, settings.Style);
        Assert.Equal("#00FF00", settings.Color);
    }
}
