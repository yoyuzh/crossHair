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
    public bool ComplianceNoticeAccepted { get; set; }
    public string ComplianceNoticeVersion { get; set; } = Compliance.ComplianceNotice.Version;

    public static CrosshairSettings CreateDefault() => new();
}
