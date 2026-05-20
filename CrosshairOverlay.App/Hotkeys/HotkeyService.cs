using System.Windows.Interop;
using CrosshairOverlay.App.Native;

namespace CrosshairOverlay.App.Hotkeys;

public sealed class HotkeyService : IDisposable
{
    private readonly HwndSource source;
    private readonly Dictionary<int, Action> actions = [];
    private readonly HashSet<int> registeredIds = [];

    public HotkeyService(HwndSource source)
    {
        this.source = source;
        source.AddHook(WndProc);
    }

    public void RegisterHotkeys(CrosshairOverlay.App.Models.CrosshairSettings settings, Action toggleOverlay, Action nextProfile, Action<int> selectProfile)
    {
        ClearRegistrations();

        Register(1, settings.ToggleHotkey, toggleOverlay);
        Register(2, settings.NextProfileHotkey, nextProfile);
        for (var i = 0; i < 4; i++)
        {
            var index = i;
            var hotkey = i < settings.ProfileHotkeys.Length ? settings.ProfileHotkeys[i] : string.Empty;
            Register(10 + i, hotkey, () => selectProfile(index));
        }
    }

    public void Dispose()
    {
        source.RemoveHook(WndProc);
        ClearRegistrations();
    }

    private void Register(int id, string hotkey, Action action)
    {
        if (!HotkeyGesture.TryParse(hotkey, out var gesture))
        {
            return;
        }

        if (Win32.RegisterHotKey(source.Handle, id, gesture.Modifiers, gesture.VirtualKey))
        {
            registeredIds.Add(id);
            actions[id] = action;
        }
    }

    private void ClearRegistrations()
    {
        foreach (var id in registeredIds)
        {
            Win32.UnregisterHotKey(source.Handle, id);
        }

        registeredIds.Clear();
        actions.Clear();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Win32.WmHotkey && actions.TryGetValue(wParam.ToInt32(), out var action))
        {
            handled = true;
            action();
        }

        return IntPtr.Zero;
    }
}
