using CrosshairOverlay.App.Native;

namespace CrosshairOverlay.App.Hotkeys;

public sealed record HotkeyGesture(uint Modifiers, uint VirtualKey)
{
    public static bool TryParse(string value, out HotkeyGesture gesture)
    {
        gesture = new HotkeyGesture(Win32.ModNorepeat, 0);
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        uint modifiers = Win32.ModNorepeat;
        uint virtualKey = 0;
        var parts = value.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var part in parts)
        {
            switch (part.ToUpperInvariant())
            {
                case "CTRL":
                case "CONTROL":
                    modifiers |= Win32.ModControl;
                    break;
                case "ALT":
                    modifiers |= Win32.ModAlt;
                    break;
                case "SHIFT":
                    modifiers |= Win32.ModShift;
                    break;
                case "WIN":
                case "WINDOWS":
                    modifiers |= Win32.ModWin;
                    break;
                default:
                    if (!TryParseKey(part, out virtualKey))
                    {
                        return false;
                    }
                    break;
            }
        }

        if (virtualKey == 0)
        {
            return false;
        }

        gesture = new HotkeyGesture(modifiers, virtualKey);
        return true;
    }

    private static bool TryParseKey(string value, out uint virtualKey)
    {
        virtualKey = 0;
        var normalized = value.ToUpperInvariant();

        if (normalized.Length == 1)
        {
            var c = normalized[0];
            if (c is >= 'A' and <= 'Z' or >= '0' and <= '9')
            {
                virtualKey = c;
                return true;
            }
        }

        if (normalized.StartsWith("F", StringComparison.Ordinal) &&
            int.TryParse(normalized[1..], out var functionKey) &&
            functionKey is >= 1 and <= 24)
        {
            virtualKey = (uint)(0x70 + functionKey - 1);
            return true;
        }

        return false;
    }
}
