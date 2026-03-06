// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TempConvPro.Models;
using TempConvPro.Services;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TempConvPro.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase, IAsyncDisposable
    {
        // Conversion constants to eliminate magic numbers
        private const double AbsoluteZeroCelsius = -273.15;
        private const double CelsiusToKelvinOffset = 273.15;
        private const double CelsiusToFahrenheitFactor = 9d / 5d;
        private const double FahrenheitOffset = 32;
        private const double FahrenheitToRankineOffset = 459.67;
        private const double CelsiusToReaumurFactor = 4d / 5d;
        private const double CelsiusToRomerFactor = 21d / 40d;
        private const double RomerOffset = 7.5;
        private const double CelsiusToNewtonFactor = 33d / 100d;
        private const double DelisleBase = 100d;
        private const double CelsiusToDelisleFactor = 3d / 2d;

        private readonly IClipboardService _clipboardService;
        private readonly ISettingsService _settingsService;
        private readonly IFileService _fileService;
        private bool _isLoadingSettings = false;
        private CancellationTokenSource? _statusMessageCancellation;
        private CancellationTokenSource? _autoSaveCancellation;

        [ObservableProperty]
        private string _celsius = "0";

        [ObservableProperty]
        private string _fahrenheit = "32";

        [ObservableProperty]
        private string _kelvin = "273.15";

        [ObservableProperty]
        private string _rankine = "491.67";

        // Historical scales
        [ObservableProperty]
        private string _reaumur = "0";

        [ObservableProperty]
        private string _romer = "7.5";

        [ObservableProperty]
        private string _newton = "0";

        [ObservableProperty]
        private string _delisle = "150";

        [ObservableProperty]
        private bool _showAbsoluteZeroWarning = false;

        [ObservableProperty]
        private string _warningMessage = string.Empty;

        [ObservableProperty]
        private bool _autoSaveSettings = true;

        [ObservableProperty]
        private bool _restoreLastValues = true;

        [ObservableProperty]
        private string _statusMessage = "Ready";

        [ObservableProperty]
        private bool _isStatusVisible = false;

        [ObservableProperty]
        private TemperaturePreset? _selectedPreset;

        [ObservableProperty]
        private bool _showHistoricalScales = false;

        // Fixed decimal places - always show 2 decimal places
        private int DecimalPlaces => 2;

        public ObservableCollection<string> ConversionHistory { get; } = new();

        public ObservableCollection<TemperaturePreset> TemperaturePresets { get; } = new();

        public MainWindowViewModel(IClipboardService clipboardService, ISettingsService settingsService, IFileService fileService)
        {
            _clipboardService = clipboardService;
            _settingsService = settingsService;
            _fileService = fileService;
            InitializePresets();
        }

        private void InitializePresets()
        {
            TemperaturePresets.Add(new TemperaturePreset("Absolute Zero", "❄️", -273.15, "Coldest possible temperature"));
            TemperaturePresets.Add(new TemperaturePreset("Water Freezing", "🧊", 0, "Water freezes at 1 atm"));
            TemperaturePresets.Add(new TemperaturePreset("Room Temperature", "🏠", 20, "Comfortable indoor temp"));
            TemperaturePresets.Add(new TemperaturePreset("Body Temperature", "🌡️", 37, "Normal human body temp"));
            TemperaturePresets.Add(new TemperaturePreset("Hot Summer Day", "☀️", 38, "Very hot outdoor temp"));
            TemperaturePresets.Add(new TemperaturePreset("Water Boiling", "🔥", 100, "Water boils at 1 atm"));
            TemperaturePresets.Add(new TemperaturePreset("Oven Baking", "🍞", 180, "Typical baking temperature"));
            TemperaturePresets.Add(new TemperaturePreset("Surface of Sun", "☀️", 5500, "Approximate surface temp"));
        }

        public async Task InitializeAsync()
        {
            await LoadSettingsAsync();
            ShowStatusMessage("Settings loaded", 2);
        }

        partial void OnCelsiusChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double c))
            {
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);

                // Auto-save when value changes (debounced)
                if (AutoSaveSettings && !_isLoadingSettings)
                {
                    _ = DebouncedSaveAsync();
                }
            }
        }

        partial void OnFahrenheitChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double f))
            {
                var c = (f - FahrenheitOffset) / CelsiusToFahrenheitFactor;
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);

                // Auto-save when value changes (debounced)
                if (AutoSaveSettings && !_isLoadingSettings)
                {
                    _ = DebouncedSaveAsync();
                }
            }
        }

        partial void OnKelvinChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double k))
            {
                var c = k - CelsiusToKelvinOffset;
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);

                // Auto-save when value changes (debounced)
                if (AutoSaveSettings && !_isLoadingSettings)
                {
                    _ = DebouncedSaveAsync();
                }
            }
        }

        private async Task DebouncedSaveAsync()
        {
            // Cancel any pending save
            _autoSaveCancellation?.Cancel();
            _autoSaveCancellation?.Dispose();
            _autoSaveCancellation = new CancellationTokenSource();

            try
            {
                // Save after 500ms of no changes
                await Task.Delay(500, _autoSaveCancellation.Token);
                await SaveCurrentSettingsAsync();
            }
            catch (OperationCanceledException)
            {
                // Expected when a new edit occurs before delay completes
            }
        }

        partial void OnRankineChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double r))
            {
                var c = (r / CelsiusToFahrenheitFactor) - CelsiusToKelvinOffset;
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);
            }
        }

        partial void OnReaumurChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double re))
            {
                var c = re / CelsiusToReaumurFactor;
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);
            }
        }

        partial void OnRomerChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double ro))
            {
                var c = (ro - RomerOffset) / CelsiusToRomerFactor;
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);
            }
        }

        partial void OnNewtonChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double n))
            {
                var c = n / CelsiusToNewtonFactor;
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);
            }
        }

        partial void OnDelisleChanged(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double de))
            {
                var c = DelisleBase - (de / CelsiusToDelisleFactor);
                CheckAbsoluteZero(c);
                UpdateAllScalesFromCelsius(c);
            }
        }

        // Suppress warnings - must use backing fields to prevent infinite loops
        #pragma warning disable MVVMTK0034
        /// <summary>
        /// Unified method to update all temperature scales from Celsius value.
        /// This eliminates code duplication from the previous 8 separate UpdateFrom* methods.
        /// </summary>
        private void UpdateAllScalesFromCelsius(double c)
        {
            // Convert from Celsius to all scales
            var f = c * CelsiusToFahrenheitFactor + FahrenheitOffset;
            var k = c + CelsiusToKelvinOffset;
            var r = (c + CelsiusToKelvinOffset) * CelsiusToFahrenheitFactor;

            // Historical scales
            var re = c * CelsiusToReaumurFactor;                    // Réaumur
            var ro = c * CelsiusToRomerFactor + RomerOffset;        // Rømer
            var n = c * CelsiusToNewtonFactor;                      // Newton
            var de = (DelisleBase - c) * CelsiusToDelisleFactor;    // Delisle (inverted!)

            var format = GetFormat();
            _celsius = c.ToString(format, CultureInfo.InvariantCulture);
            _fahrenheit = f.ToString(format, CultureInfo.InvariantCulture);
            _kelvin = k.ToString(format, CultureInfo.InvariantCulture);
            _rankine = r.ToString(format, CultureInfo.InvariantCulture);
            _reaumur = re.ToString(format, CultureInfo.InvariantCulture);
            _romer = ro.ToString(format, CultureInfo.InvariantCulture);
            _newton = n.ToString(format, CultureInfo.InvariantCulture);
            _delisle = de.ToString(format, CultureInfo.InvariantCulture);

            OnPropertyChanged(nameof(Celsius));
            OnPropertyChanged(nameof(Fahrenheit));
            OnPropertyChanged(nameof(Kelvin));
            OnPropertyChanged(nameof(Rankine));
            OnPropertyChanged(nameof(Reaumur));
            OnPropertyChanged(nameof(Romer));
            OnPropertyChanged(nameof(Newton));
            OnPropertyChanged(nameof(Delisle));

            AddToHistory($"{c.ToString($"F{DecimalPlaces}")}°C = {f.ToString($"F{DecimalPlaces}")}°F = {k.ToString($"F{DecimalPlaces}")}K = {r.ToString($"F{DecimalPlaces}")}°R");
        }
        #pragma warning restore MVVMTK0034

        private void CheckAbsoluteZero(double celsius)
        {
            if (celsius < AbsoluteZeroCelsius)
            {
                ShowAbsoluteZeroWarning = true;
                WarningMessage = $"⚠️ Warning: Temperature is below absolute zero ({AbsoluteZeroCelsius}°C)!";
            }
            else
            {
                ShowAbsoluteZeroWarning = false;
                WarningMessage = string.Empty;
            }
        }

        private string GetFormat()
        {
            return DecimalPlaces == 0 ? "0" : $"0.{new string('0', DecimalPlaces)}";
        }

        private void AddToHistory(string conversion)
        {
            ConversionHistory.Insert(0, conversion);
            if (ConversionHistory.Count > 10)
            {
                ConversionHistory.RemoveAt(10);
            }
        }

        [RelayCommand]
        private void ClearAll()
        {
            Celsius = "0";
            Fahrenheit = "32";
            Kelvin = "273.15";
            Rankine = "491.67";
            Reaumur = "0";
            Romer = "7.5";
            Newton = "0";
            Delisle = "150";

            ShowAbsoluteZeroWarning = false;
            WarningMessage = string.Empty;

            ShowStatusMessage("All values cleared", 2);
        }

        [RelayCommand]
        private async Task SaveAsCustomPreset()
        {
            if (!double.TryParse(Celsius, NumberStyles.Float, CultureInfo.InvariantCulture, out double c))
                return;

            var preset = new TemperaturePreset
            {
                Name = $"Custom {c}°C",
                Icon = "⭐",
                Celsius = c,
                Description = "User-created preset",
                IsCustom = true
            };

            TemperaturePresets.Add(preset);
            await SaveCustomPresetsAsync();
            ShowStatusMessage($"⭐ Saved as custom preset", 3);
        }

        private async Task SaveCustomPresetsAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();
            var customPresets = TemperaturePresets.Where(p => p.IsCustom).ToList();

            // Save custom presets as JSON in settings
            // For now, just a placeholder - would need to add to AppSettings
        }

        [RelayCommand]
        private void ClearHistory()
        {
            ConversionHistory.Clear();
            ShowStatusMessage("History cleared", 2);
        }

        [RelayCommand]
        private async Task CopyToClipboard()
        {
            var text = GetFormattedConversion();
            if (!string.IsNullOrEmpty(text))
            {
                await _clipboardService.SetTextAsync(text);
                ShowStatusMessage("📋 Copied to clipboard!", 3);
            }
        }

        [RelayCommand]
        private async Task SaveSettings()
        {
            await SaveCurrentSettingsAsync();
            ShowStatusMessage("💾 Settings saved", 2);
        }

        [RelayCommand]
        private async Task ResetSettings()
        {
            await _settingsService.ResetSettingsAsync();
            await LoadSettingsAsync();
            ShowStatusMessage("🔄 Settings reset to defaults", 3);
        }

        [RelayCommand]
        private void ApplyPreset()
        {
            if (SelectedPreset != null)
            {
                _isLoadingSettings = true; // Prevent auto-save during preset application

                Celsius = SelectedPreset.Celsius.ToString($"F{DecimalPlaces}", CultureInfo.InvariantCulture);

                _isLoadingSettings = false;

                ShowStatusMessage($"{SelectedPreset.Icon} Applied: {SelectedPreset.Name}", 3);
            }
        }

        [RelayCommand]
        private async Task ExportHistoryToCsv()
        {
            if (!ConversionHistory.Any())
            {
                ShowStatusMessage("No history to export", 2);
                return;
            }

            var content = _fileService.ExportToCsv(ConversionHistory);
            var success = await _fileService.ExportToFileAsync(content, "temperature_history.csv", FileExportFormat.Csv);

            if (success)
                ShowStatusMessage("📄 Exported to CSV!", 3);
            else
                ShowStatusMessage("Export cancelled", 2);
        }

        [RelayCommand]
        private async Task ExportHistoryToJson()
        {
            if (!ConversionHistory.Any())
            {
                ShowStatusMessage("No history to export", 2);
                return;
            }

            var content = _fileService.ExportToJson(ConversionHistory);
            var success = await _fileService.ExportToFileAsync(content, "temperature_history.json", FileExportFormat.Json);

            if (success)
                ShowStatusMessage("📋 Exported to JSON!", 3);
            else
                ShowStatusMessage("Export cancelled", 2);
        }

        [RelayCommand]
        private async Task ExportHistoryToText()
        {
            if (!ConversionHistory.Any())
            {
                ShowStatusMessage("No history to export", 2);
                return;
            }

            var content = _fileService.ExportToText(ConversionHistory);
            var success = await _fileService.ExportToFileAsync(content, "temperature_history.txt", FileExportFormat.Text);

            if (success)
                ShowStatusMessage("📝 Exported to TXT!", 3);
            else
                ShowStatusMessage("Export cancelled", 2);
        }

        partial void OnSelectedPresetChanged(TemperaturePreset? value)
        {
            if (value != null)
            {
                ApplyPreset();
                // Reset selection for better UX (can select same preset again)
                SelectedPreset = null;
            }
        }

        private void ShowStatusMessage(string message, int durationSeconds = 3)
        {
            // Cancel previous timer
            _statusMessageCancellation?.Cancel();
            _statusMessageCancellation = new CancellationTokenSource();

            StatusMessage = message;
            IsStatusVisible = true;

            // Auto-hide after duration
            var cancellationToken = _statusMessageCancellation.Token;
            _ = Task.Run(async () =>
            {
                await Task.Delay(durationSeconds * 1000, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    IsStatusVisible = false;
                }
            }, cancellationToken);
        }

        private async Task LoadSettingsAsync()
        {
            try
            {
                _isLoadingSettings = true;
                var settings = await _settingsService.LoadSettingsAsync();

                // Apply settings
                AutoSaveSettings = settings.AutoSave;
                RestoreLastValues = settings.RestoreLastValues;

                if (settings.RestoreLastValues)
                {
                    Celsius = settings.LastCelsius;
                    Fahrenheit = settings.LastFahrenheit;
                    Kelvin = settings.LastKelvin;
                }
            }
            finally
            {
                _isLoadingSettings = false;
            }
        }

        public async Task SaveCurrentSettingsAsync()
        {
            if (_isLoadingSettings)
                return;

            try
            {
                var settings = new AppSettings
                {
                    LastCelsius = Celsius,
                    LastFahrenheit = Fahrenheit,
                    LastKelvin = Kelvin,
                    AutoSave = AutoSaveSettings,
                    RestoreLastValues = RestoreLastValues
                };

                var success = await _settingsService.SaveSettingsAsync(settings);

                if (!success)
                {
                    ShowStatusMessage("⚠️ Failed to save settings", 4);
                    System.Diagnostics.Debug.WriteLine($"Settings save failed. Path: {_settingsService.GetSettingsFilePath()}");
                }
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"❌ Save error: {ex.Message}", 5);
                System.Diagnostics.Debug.WriteLine($"Error in SaveCurrentSettingsAsync: {ex}");
            }
        }

        partial void OnAutoSaveSettingsChanged(bool value)
        {
            if (value && !_isLoadingSettings)
            {
                _ = SaveCurrentSettingsAsync();
            }
        }

        partial void OnRestoreLastValuesChanged(bool value)
        {
            if (AutoSaveSettings && !_isLoadingSettings)
            {
                _ = SaveCurrentSettingsAsync();
            }
        }

        public string GetFormattedConversion()
        {
            if (double.TryParse(Celsius, NumberStyles.Float, CultureInfo.InvariantCulture, out double c) &&
                double.TryParse(Fahrenheit, NumberStyles.Float, CultureInfo.InvariantCulture, out double f) &&
                double.TryParse(Kelvin, NumberStyles.Float, CultureInfo.InvariantCulture, out double k) &&
                double.TryParse(Rankine, NumberStyles.Float, CultureInfo.InvariantCulture, out double r))
            {
                return $"{c.ToString($"F{DecimalPlaces}")}°C = {f.ToString($"F{DecimalPlaces}")}°F = {k.ToString($"F{DecimalPlaces}")}K = {r.ToString($"F{DecimalPlaces}")}°R";
            }
            return string.Empty;
        }

        /// <summary>
        /// Dispose pattern to clean up cancellation tokens and prevent memory leaks
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            // Cancel any pending operations
            _statusMessageCancellation?.Cancel();
            _autoSaveCancellation?.Cancel();

            // Give a moment for any in-flight saves to complete
            try
            {
                await Task.Delay(100);
            }
            catch (OperationCanceledException)
            {
                // Expected
            }

            // Dispose cancellation tokens
            _statusMessageCancellation?.Dispose();
            _autoSaveCancellation?.Dispose();
        }
    }
}
