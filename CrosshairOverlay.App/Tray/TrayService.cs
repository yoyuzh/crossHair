using System.Windows;
using Forms = System.Windows.Forms;

namespace CrosshairOverlay.App.Tray;

public sealed class TrayService : IDisposable
{
    private readonly Forms.NotifyIcon notifyIcon;

    public TrayService(Window settingsWindow, Action toggleOverlay)
    {
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add("打开设置", null, (_, _) => ShowSettings(settingsWindow));
        menu.Items.Add("显示/隐藏准星", null, (_, _) => toggleOverlay());
        menu.Items.Add("退出", null, (_, _) => System.Windows.Application.Current.Shutdown());

        notifyIcon = new Forms.NotifyIcon
        {
            Text = "外置准星",
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
