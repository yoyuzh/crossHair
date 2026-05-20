namespace CrosshairOverlay.App.Models;

public sealed class CrosshairSettings
{
    public bool Enabled { get; set; } = true;
    public CrosshairStyle Style { get; set; } = CrosshairStyle.CrossDot;
    public double Size { get; set; } = 16;
    public double Thickness { get; set; } = 2;
    public double Gap { get; set; } = 4;
    public double Opacity { get; set; } = 0.9;
    public string Color { get; set; } = "#00FF00";
    public double OffsetX { get; set; }
    public double OffsetY { get; set; }
    public string MonitorId { get; set; } = "Primary";
    public string ToggleHotkey { get; set; } = "F8";
    public string NextProfileHotkey { get; set; } = "F9";
    public string[] ProfileHotkeys { get; set; } = ["Ctrl+Alt+1", "Ctrl+Alt+2", "Ctrl+Alt+3", "Ctrl+Alt+4"];
    public int ActiveProfileIndex { get; set; }
    public List<CrosshairProfile> Profiles { get; set; } =
    [
        new() { Name = "方案 1", Style = CrosshairStyle.CrossDot, Color = "#00FF00", Size = 16, Thickness = 2, Gap = 4, Opacity = 0.9 },
        new() { Name = "方案 2", Style = CrosshairStyle.Dot, Color = "#FFFFFF", Size = 16, Thickness = 3, Gap = 4, Opacity = 0.9 },
        new() { Name = "方案 3", Style = CrosshairStyle.Cross, Color = "#FFCC00", Size = 18, Thickness = 2, Gap = 5, Opacity = 0.9 },
        new() { Name = "方案 4", Style = CrosshairStyle.Circle, Color = "#0A84FF", Size = 14, Thickness = 2, Gap = 4, Opacity = 0.85 }
    ];
    public bool ComplianceNoticeAccepted { get; set; }
    public string ComplianceNoticeVersion { get; set; } = Compliance.ComplianceNotice.Version;

    public static CrosshairSettings CreateDefault() => new();
}
