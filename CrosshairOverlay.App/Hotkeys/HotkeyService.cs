using System.Windows.Interop;
using CrosshairOverlay.App.Native;

namespace CrosshairOverlay.App.Hotkeys;

public sealed class HotkeyService : IDisposable
{
    private const int ToggleHotkeyId = 1;
    private readonly HwndSource source;
    private bool registered;

    public HotkeyService(HwndSource source)
    {
        this.source = source;
        source.AddHook(WndProc);
    }

    public event EventHandler? ToggleRequested;

    public void RegisterToggleHotkey()
    {
        registered = Win32.RegisterHotKey(source.Handle, ToggleHotkeyId, Win32.ModNorepeat, Win32.VkF8);
    }

    public void Dispose()
    {
        source.RemoveHook(WndProc);
        if (registered)
        {
            Win32.UnregisterHotKey(source.Handle, ToggleHotkeyId);
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Win32.WmHotkey && wParam.ToInt32() == ToggleHotkeyId)
        {
            handled = true;
            ToggleRequested?.Invoke(this, EventArgs.Empty);
        }

        return IntPtr.Zero;
    }
}
