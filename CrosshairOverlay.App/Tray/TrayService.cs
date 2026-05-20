using System.Windows;
using Forms = System.Windows.Forms;

namespace CrosshairOverlay.App.Tray;

public sealed class TrayService : IDisposable
{
    private readonly Forms.NotifyIcon notifyIcon;

    public TrayService(Window settingsWindow, Action toggleOverlay)
    {
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("Open Settings", null, (_, _) => ShowSettings(settingsWindow));
        menu.Items.Add("Toggle Crosshair", null, (_, _) => toggleOverlay());
        menu.Items.Add("Exit", null, (_, _) => Application.Current.Shutdown());

        notifyIcon = new Forms.NotifyIcon
        {
            Text = "Crosshair Overlay",
            Icon = System.Drawing.SystemIcons.Application,
            ContextMenuStrip = menu,
            Visible = true
        };

        notifyIcon.DoubleClick += (_, _) => ShowSettings(settingsWindow);
    }

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Dispose();
    }

    private static void ShowSettings(Window settingsWindow)
    {
        if (!settingsWindow.IsVisible)
        {
            settingsWindow.Show();
        }

        if (settingsWindow.WindowState == WindowState.Minimized)
        {
            settingsWindow.WindowState = WindowState.Normal;
        }

        settingsWindow.Activate();
    }
}
