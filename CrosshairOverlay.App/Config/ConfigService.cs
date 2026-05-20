using System.IO;
using System.Text.Json;
using CrosshairOverlay.App.Models;

namespace CrosshairOverlay.App.Config;

public sealed class ConfigService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly string configPath;

    public ConfigService(string configPath)
    {
        this.configPath = configPath;
    }

    public static string GetDefaultConfigPath()
    {
        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appData, "CrosshairOverlay", "config.json");
    }

    public CrosshairSettings Load()
    {
        if (!File.Exists(configPath))
        {
            return CrosshairSettings.CreateDefault();
        }

        try
        {
            var json = File.ReadAllText(configPath);
            var settings = JsonSerializer.Deserialize<CrosshairSettings>(json, SerializerOptions);
            return settings is null ? CrosshairSettings.CreateDefault() : Normalize(settings);
        }
        catch (JsonException)
        {
            return CrosshairSettings.CreateDefault();
        }
        catch (IOException)
        {
            return CrosshairSettings.CreateDefault();
        }
    }

    public void Save(CrosshairSettings settings)
    {
        var directory = Path.GetDirectoryName(configPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(Normalize(settings), SerializerOptions);
        File.WriteAllText(configPath, json);
    }

    private static CrosshairSettings Normalize(CrosshairSettings settings)
    {
        settings.Size = Clamp(settings.Size, 1, 100);
        settings.Thickness = Clamp(settings.Thickness, 1, 20);
        settings.Gap = Clamp(settings.Gap, 0, 100);
        settings.Opacity = Clamp(settings.Opacity, 0.1, 1);
        settings.Color = string.IsNullOrWhiteSpace(settings.Color) ? "#00FF00" : settings.Color;
        settings.MonitorId = string.IsNullOrWhiteSpace(settings.MonitorId) ? "Primary" : settings.MonitorId;
        settings.ToggleHotkey = string.IsNullOrWhiteSpace(settings.ToggleHotkey) ? "F8" : settings.ToggleHotkey;
        settings.ComplianceNoticeVersion = string.IsNullOrWhiteSpace(settings.ComplianceNoticeVersion)
            ? Compliance.ComplianceNotice.Version
            : settings.ComplianceNoticeVersion;
        return settings;
    }

    private static double Clamp(double value, double min, double max) => Math.Min(Math.Max(value, min), max);
}
