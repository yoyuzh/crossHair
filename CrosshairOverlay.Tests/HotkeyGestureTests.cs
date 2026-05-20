using CrosshairOverlay.App.Hotkeys;
using Xunit;

namespace CrosshairOverlay.Tests;

public sealed class HotkeyGestureTests
{
    [Fact]
    public void ParsesFunctionKey()
    {
        Assert.True(HotkeyGesture.TryParse("F9", out var gesture));
        Assert.Equal(0x4000u, gesture.Modifiers);
        Assert.Equal(0x78u, gesture.VirtualKey);
    }

    [Fact]
    public void ParsesModifierAndNumber()
    {
        Assert.True(HotkeyGesture.TryParse("Ctrl+Alt+3", out var gesture));
        Assert.Equal(0x4000u | 0x0002u | 0x0001u, gesture.Modifiers);
        Assert.Equal((uint)'3', gesture.VirtualKey);
    }

    [Fact]
    public void RejectsMissingKey()
    {
        Assert.False(HotkeyGesture.TryParse("Ctrl+Alt", out _));
    }
}
