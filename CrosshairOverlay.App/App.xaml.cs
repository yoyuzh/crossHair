using System.Windows;
using System.Windows.Interop;
using CrosshairOverlay.App.Compliance;
using CrosshairOverlay.App.Config;
using CrosshairOverlay.App.Hotkeys;
using CrosshairOverlay.App.Tray;

namespace CrosshairOverlay.App;

public partial class App : Application
{
    private ConfigService? configService;
    private OverlayWindow? overlayWindow;
    private MainWindow? mainWindow;
    private HotkeyService? hotkeyService;
    private TrayService? trayService;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        configService = new ConfigService(ConfigService.GetDefaultConfigPath());
        var settings = configService.Load();

        if (ComplianceNotice.RequiresAcceptance(settings.ComplianceNoticeVersion, settings.ComplianceNoticeAccepted))
        {
            var result = MessageBox.Show(
                ComplianceNotice.Body,
                ComplianceNotice.Title,
                MessageBoxButton.OKCancel,
                MessageBoxImage.Information);

            if (result != MessageBoxResult.OK)
            {
                Shutdown();
                return;
            }

            settings.ComplianceNoticeAccepted = true;
            settings.ComplianceNoticeVersion = ComplianceNotice.Version;
            configService.Save(settings);
        }

        overlayWindow = new OverlayWindow();
        overlayWindow.FitPrimaryScreen();
        overlayWindow.Show();
        overlayWindow.ApplySettings(settings);

        mainWindow = new MainWindow(configService, overlayWindow, settings);
        mainWindow.Closing += (_, args) =>
        {
            args.Cancel = true;
            mainWindow.Hide();
        };
        mainWindow.Show();

        var source = HwndSource.FromHwnd(new WindowInteropHelper(mainWindow).Handle);
        if (source is null)
        {
            MessageBox.Show("Unable to initialize global hotkey window source.", "Crosshair Overlay", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        hotkeyService = new HotkeyService(source);
        hotkeyService.ToggleRequested += (_, _) => mainWindow.ToggleOverlay();
        hotkeyService.RegisterToggleHotkey();

        trayService = new TrayService(mainWindow, mainWindow.ToggleOverlay);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        hotkeyService?.Dispose();
        trayService?.Dispose();
        base.OnExit(e);
    }
}
