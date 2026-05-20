using System.Runtime.InteropServices;

namespace CrosshairOverlay.App.Native;

internal static class Win32
{
    public const int GwlExstyle = -20;
    public const int WsExTransparent = 0x00000020;
    public const int WsExLayered = 0x00080000;
    public const int WsExToolwindow = 0x00000080;
    public const int WsExNoactivate = 0x08000000;
    public const int WmNchittest = 0x0084;
    public const int Httransparent = -1;
    public const int WmHotkey = 0x0312;
    public const uint ModAlt = 0x0001;
    public const uint ModControl = 0x0002;
    public const uint ModShift = 0x0004;
    public const uint ModWin = 0x0008;
    public const uint ModNorepeat = 0x4000;
    public const uint VkF8 = 0x77;

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public static void ApplyOverlayExtendedStyles(IntPtr hWnd)
    {
        var current = GetWindowLongPtr(hWnd, GwlExstyle).ToInt64();
        var updated = current | WsExLayered | WsExTransparent | WsExNoactivate | WsExToolwindow;
        SetWindowLongPtr(hWnd, GwlExstyle, new IntPtr(updated));
    }
}
