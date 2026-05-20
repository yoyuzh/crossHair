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
        settings.NextProfileHotkey = string.IsNullOrWhiteSpace(settings.NextProfileHotkey) ? "F9" : settings.NextProfileHotkey;
        settings.ProfileHotkeys = NormalizeProfileHotkeys(settings.ProfileHotkeys);
        settings.Profiles = NormalizeProfiles(settings);
        settings.ActiveProfileIndex = (int)Clamp(settings.ActiveProfileIndex, 0, settings.Profiles.Count - 1);
        settings.ComplianceNoticeVersion = string.IsNullOrWhiteSpace(settings.ComplianceNoticeVersion)
            ? Compliance.ComplianceNotice.Version
            : settings.ComplianceNoticeVersion;
        return settings;
    }

    private static double Clamp(double value, double min, double max) => Math.Min(Math.Max(value, min), max);

    private static string[] NormalizeProfileHotkeys(string[]? hotkeys)
    {
        var defaults = new[] { "Ctrl+Alt+1", "Ctrl+Alt+2", "Ctrl+Alt+3", "Ctrl+Alt+4" };
        var normalized = new string[4];
        for (var i = 0; i < normalized.Length; i++)
        {
            normalized[i] = hotkeys is { Length: > 0 } && i < hotkeys.Length && !string.IsNullOrWhiteSpace(hotkeys[i])
                ? hotkeys[i]
                : defaults[i];
        }

        return normalized;
    }

    private static List<CrosshairProfile> NormalizeProfiles(CrosshairSettings settings)
    {
        var profiles = settings.Profiles ?? [];
        var defaults = CrosshairSettings.CreateDefault().Profiles;
        var normalized = new List<CrosshairProfile>(4);
        for (var i = 0; i < 4; i++)
        {
            var profile = i < profiles.Count && profiles[i] is not null ? profiles[i] : defaults[i];
            profile.Name = string.IsNullOrWhiteSpace(profile.Name) ? $"Profile {i + 1}" : profile.Name;
            profile.Color = string.IsNullOrWhiteSpace(profile.Color) ? defaults[i].Color : profile.Color;
            profile.Size = Clamp(profile.Size, 1, 100);
            profile.Thickness = Clamp(profile.Thickness, 1, 20);
            profile.Gap = Clamp(profile.Gap, 0, 100);
            profile.Opacity = Clamp(profile.Opacity, 0.1, 1);
            normalized.Add(profile);
        }

        return normalized;
    }
}
