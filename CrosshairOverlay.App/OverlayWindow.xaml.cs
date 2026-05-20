using System.Windows;
using System.Windows.Interop;
using CrosshairOverlay.App.Models;
using CrosshairOverlay.App.Native;

namespace CrosshairOverlay.App;

public partial class OverlayWindow : Window
{
    public OverlayWindow()
    {
        InitializeComponent();
        SourceInitialized += OnSourceInitialized;
    }

    public void ApplySettings(CrosshairSettings settings)
    {
        Renderer.Settings = settings;
        Visibility = settings.Enabled ? Visibility.Visible : Visibility.Hidden;
        Renderer.InvalidateVisual();
    }

    public void FitPrimaryScreen()
    {
        Left = SystemParameters.VirtualScreenLeft;
        Top = SystemParameters.VirtualScreenTop;
        Width = SystemParameters.PrimaryScreenWidth;
        Height = SystemParameters.PrimaryScreenHeight;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        var helper = new WindowInteropHelper(this);
        Win32.ApplyOverlayExtendedStyles(helper.Handle);
        HwndSource.FromHwnd(helper.Handle)?.AddHook(WndProc);
    }

    private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == Win32.WmNchittest)
        {
            handled = true;
            return new IntPtr(Win32.Httransparent);
        }

        return IntPtr.Zero;
    }
}
