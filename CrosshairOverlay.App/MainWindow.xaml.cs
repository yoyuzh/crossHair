using System.Windows;
using System.Windows.Media;
using CrosshairOverlay.App.Config;
using CrosshairOverlay.App.Models;
using MediaColor = System.Windows.Media.Color;
using MediaColorConverter = System.Windows.Media.ColorConverter;

namespace CrosshairOverlay.App;

public partial class MainWindow : Window
{
    private sealed record StyleOption(CrosshairStyle Value, string Label)
    {
        public override string ToString() => Label;
    }

    private readonly ConfigService configService;
    private readonly OverlayWindow overlayWindow;
    private readonly Action? hotkeysChanged;
    private readonly StyleOption[] styleOptions =
    [
        new(CrosshairStyle.CrossDot, "十字 + 圆点"),
        new(CrosshairStyle.Dot, "圆点"),
        new(CrosshairStyle.Cross, "十字"),
        new(CrosshairStyle.Circle, "圆环")
    ];

    private bool loading;
    private CrosshairSettings settings;

    public MainWindow(ConfigService configService, OverlayWindow overlayWindow, CrosshairSettings settings, Action? hotkeysChanged = null)
    {
        InitializeComponent();
        this.configService = configService;
        this.overlayWindow = overlayWindow;
        this.settings = settings;
        this.hotkeysChanged = hotkeysChanged;

        StyleComboBox.ItemsSource = styleOptions;
        LoadControls(settings);
        ApplySettings();
    }

    public void ToggleOverlay()
    {
        settings.Enabled = !settings.Enabled;
        LoadControls(settings);
        SaveAndApply();
    }

    public void SelectProfile(int index)
    {
        if (index < 0 || index >= settings.Profiles.Count)
        {
            return;
        }

        SaveCurrentProfile();
        settings.ActiveProfileIndex = index;
        settings.Profiles[index].ApplyTo(settings);
        LoadControls(settings);
        SaveAndApply();
    }

    public void SelectNextProfile()
    {
        SelectProfile((settings.ActiveProfileIndex + 1) % settings.Profiles.Count);
    }

    private void LoadControls(CrosshairSettings value)
    {
        loading = true;
        RefreshProfileList(value.ActiveProfileIndex);
        ProfileComboBox.SelectedIndex = value.ActiveProfileIndex;
        ProfileNameTextBox.Text = value.Profiles[value.ActiveProfileIndex].Name;
        EnabledCheckBox.IsChecked = value.Enabled;
        StyleComboBox.SelectedItem = styleOptions.FirstOrDefault(option => option.Value == value.Style);
        SetColorControls(value.Color);
        SizeSlider.Value = value.Size;
        ThicknessSlider.Value = value.Thickness;
        GapSlider.Value = value.Gap;
        OpacitySlider.Value = value.Opacity;
        OffsetXTextBox.Text = value.OffsetX.ToString("0");
        OffsetYTextBox.Text = value.OffsetY.ToString("0");
        ToggleHotkeyTextBox.Text = value.ToggleHotkey;
        NextProfileHotkeyTextBox.Text = value.NextProfileHotkey;
        Profile1HotkeyTextBox.Text = value.ProfileHotkeys[0];
        Profile2HotkeyTextBox.Text = value.ProfileHotkeys[1];
        Profile3HotkeyTextBox.Text = value.ProfileHotkeys[2];
        Profile4HotkeyTextBox.Text = value.ProfileHotkeys[3];
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
        settings.Style = StyleComboBox.SelectedItem is StyleOption option ? option.Value : CrosshairStyle.CrossDot;
        settings.Color = CurrentColorHex();
        settings.Size = SizeSlider.Value;
        settings.Thickness = ThicknessSlider.Value;
        settings.Gap = GapSlider.Value;
        settings.Opacity = OpacitySlider.Value;
        settings.OffsetX = double.TryParse(OffsetXTextBox.Text, out var offsetX) ? offsetX : 0;
        settings.OffsetY = double.TryParse(OffsetYTextBox.Text, out var offsetY) ? offsetY : 0;
        settings.ToggleHotkey = string.IsNullOrWhiteSpace(ToggleHotkeyTextBox.Text) ? "F8" : ToggleHotkeyTextBox.Text.Trim();
        settings.NextProfileHotkey = string.IsNullOrWhiteSpace(NextProfileHotkeyTextBox.Text) ? "F9" : NextProfileHotkeyTextBox.Text.Trim();
        settings.ProfileHotkeys =
        [
            string.IsNullOrWhiteSpace(Profile1HotkeyTextBox.Text) ? "Ctrl+Alt+1" : Profile1HotkeyTextBox.Text.Trim(),
            string.IsNullOrWhiteSpace(Profile2HotkeyTextBox.Text) ? "Ctrl+Alt+2" : Profile2HotkeyTextBox.Text.Trim(),
            string.IsNullOrWhiteSpace(Profile3HotkeyTextBox.Text) ? "Ctrl+Alt+3" : Profile3HotkeyTextBox.Text.Trim(),
            string.IsNullOrWhiteSpace(Profile4HotkeyTextBox.Text) ? "Ctrl+Alt+4" : Profile4HotkeyTextBox.Text.Trim()
        ];
    }

    private void ApplySettings()
    {
        overlayWindow.ApplySettings(settings);
        PreviewRenderer.Settings = settings;
        PreviewRenderer.InvalidateVisual();
    }

    private void SaveAndApply()
    {
        SaveCurrentProfile();
        ApplySettings();
        configService.Save(settings);
        hotkeysChanged?.Invoke();
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

    private void OnPresetColorClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: string hex })
        {
            return;
        }

        SetColorControls(hex);
        ReadControls();
        ApplySettings();
    }

    private void OnColorSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (loading || !IsLoaded)
        {
            return;
        }

        UpdateColorReadout();
        ReadControls();
        ApplySettings();
    }

    private void SetColorControls(string hex)
    {
        var color = TryParseHexColor(hex, out var parsed) ? parsed : Colors.Lime;
        RedSlider.Value = color.R;
        GreenSlider.Value = color.G;
        BlueSlider.Value = color.B;
        UpdateColorReadout();
    }

    private void UpdateColorReadout()
    {
        var color = MediaColor.FromRgb(ToByte(RedSlider.Value), ToByte(GreenSlider.Value), ToByte(BlueSlider.Value));
        var hex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        ColorTextBox.Text = hex;
        ColorPreview.Background = new SolidColorBrush(color);
        RedValueText.Text = color.R.ToString();
        GreenValueText.Text = color.G.ToString();
        BlueValueText.Text = color.B.ToString();
    }

    private string CurrentColorHex()
    {
        var color = MediaColor.FromRgb(ToByte(RedSlider.Value), ToByte(GreenSlider.Value), ToByte(BlueSlider.Value));
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static byte ToByte(double value)
    {
        return (byte)Math.Clamp((int)Math.Round(value), 0, 255);
    }

    private void OnProfileSelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (loading || !IsLoaded)
        {
            return;
        }

        SelectProfile(ProfileComboBox.SelectedIndex);
    }

    private void OnSaveProfileClick(object sender, RoutedEventArgs e)
    {
        ReadControls();
        SaveAndApply();
    }

    private void OnProfileNameChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (loading || !IsLoaded)
        {
            return;
        }

        settings.Profiles[settings.ActiveProfileIndex].Name = string.IsNullOrWhiteSpace(ProfileNameTextBox.Text)
            ? $"方案 {settings.ActiveProfileIndex + 1}"
            : ProfileNameTextBox.Text.Trim();
        RefreshProfileList(settings.ActiveProfileIndex);
    }

    private void OnHotkeyChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (loading || !IsLoaded)
        {
            return;
        }

        ReadControls();
        configService.Save(settings);
        hotkeysChanged?.Invoke();
    }

    private void SaveCurrentProfile()
    {
        if (settings.ActiveProfileIndex < 0 || settings.ActiveProfileIndex >= settings.Profiles.Count)
        {
            settings.ActiveProfileIndex = 0;
        }

        var name = string.IsNullOrWhiteSpace(ProfileNameTextBox.Text)
            ? settings.Profiles[settings.ActiveProfileIndex].Name
            : ProfileNameTextBox.Text.Trim();
        settings.Profiles[settings.ActiveProfileIndex] = CrosshairProfile.FromSettings(settings, name);
    }

    private void RefreshProfileList(int selectedIndex)
    {
        var wasLoading = loading;
        loading = true;
        ProfileComboBox.ItemsSource = settings.Profiles.Select(profile => profile.Name).ToArray();
        ProfileComboBox.SelectedIndex = Math.Clamp(selectedIndex, 0, settings.Profiles.Count - 1);
        loading = wasLoading;
    }

    private static bool TryParseHexColor(string value, out MediaColor color)
    {
        color = Colors.Lime;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        if (!normalized.StartsWith("#", StringComparison.Ordinal))
        {
            normalized = "#" + normalized;
        }

        try
        {
            var converted = MediaColorConverter.ConvertFromString(normalized);
            if (converted is MediaColor parsed)
            {
                color = parsed;
                return true;
            }
        }
        catch (FormatException)
        {
            return false;
        }

        return false;
    }
}
