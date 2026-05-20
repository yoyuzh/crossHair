using System.Windows;
using CrosshairOverlay.App.Config;
using CrosshairOverlay.App.Models;

namespace CrosshairOverlay.App;

public partial class MainWindow : Window
{
    private readonly ConfigService configService;
    private readonly OverlayWindow overlayWindow;
    private bool loading;
    private CrosshairSettings settings;

    public MainWindow(ConfigService configService, OverlayWindow overlayWindow, CrosshairSettings settings)
    {
        InitializeComponent();
        this.configService = configService;
        this.overlayWindow = overlayWindow;
        this.settings = settings;

        StyleComboBox.ItemsSource = Enum.GetValues(typeof(CrosshairStyle));
        LoadControls(settings);
        ApplySettings();
    }

    public void ToggleOverlay()
    {
        settings.Enabled = !settings.Enabled;
        LoadControls(settings);
        SaveAndApply();
    }

    private void LoadControls(CrosshairSettings value)
    {
        loading = true;
        EnabledCheckBox.IsChecked = value.Enabled;
        StyleComboBox.SelectedItem = value.Style;
        ColorTextBox.Text = value.Color;
        SizeSlider.Value = value.Size;
        ThicknessSlider.Value = value.Thickness;
        GapSlider.Value = value.Gap;
        OpacitySlider.Value = value.Opacity;
        OffsetXTextBox.Text = value.OffsetX.ToString("0");
        OffsetYTextBox.Text = value.OffsetY.ToString("0");
        loading = false;
    }

    private void OnSettingChanged(object sender, RoutedEventArgs e)
    {
        if (loading || !IsLoaded)
        {
            return;
        }

        ReadControls();
        ApplySettings();
    }

    private void OnSettingChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (loading || !IsLoaded)
        {
            return;
        }

        ReadControls();
        ApplySettings();
    }

    private void ReadControls()
    {
        settings.Enabled = EnabledCheckBox.IsChecked == true;
        settings.Style = StyleComboBox.SelectedItem is CrosshairStyle style ? style : CrosshairStyle.CrossDot;
        settings.Color = string.IsNullOrWhiteSpace(ColorTextBox.Text) ? "#00FF00" : ColorTextBox.Text;
        settings.Size = SizeSlider.Value;
        settings.Thickness = ThicknessSlider.Value;
        settings.Gap = GapSlider.Value;
        settings.Opacity = OpacitySlider.Value;
        settings.OffsetX = double.TryParse(OffsetXTextBox.Text, out var offsetX) ? offsetX : 0;
        settings.OffsetY = double.TryParse(OffsetYTextBox.Text, out var offsetY) ? offsetY : 0;
    }

    private void ApplySettings()
    {
        overlayWindow.ApplySettings(settings);
    }

    private void SaveAndApply()
    {
        ApplySettings();
        configService.Save(settings);
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        ReadControls();
        SaveAndApply();
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        settings = CrosshairSettings.CreateDefault();
        settings.ComplianceNoticeAccepted = true;
        LoadControls(settings);
        SaveAndApply();
    }

    private void OnExitClick(object sender, RoutedEventArgs e)
    {
        System.Windows.Application.Current.Shutdown();
    }
}
