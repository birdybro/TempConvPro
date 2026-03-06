// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TempConvPro.Models;

namespace TempConvPro.Services
{
    /// <summary>
    /// Service for persisting application settings
    /// Pure MVVM - ViewModel has no knowledge of file system
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Load settings from storage
        /// </summary>
        Task<AppSettings> LoadSettingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Save settings to storage
        /// </summary>
        Task<bool> SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reset to default settings
        /// </summary>
        Task ResetSettingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get the settings file path
        /// </summary>
        string GetSettingsFilePath();
    }

    /// <summary>
    /// JSON-based settings service that saves to local AppData
    /// </summary>
    public class JsonSettingsService : ISettingsService
    {
        private readonly string _settingsDirectory;
        private readonly string _settingsFilePath;
        private const string SettingsFileName = "settings.json";

        public JsonSettingsService()
        {
            // Save to AppData/Roaming/TemperatureConverterPro/
            _settingsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "TemperatureConverterPro");

            _settingsFilePath = Path.Combine(_settingsDirectory, SettingsFileName);

            // Ensure directory exists
            Directory.CreateDirectory(_settingsDirectory);
        }

        public async Task<AppSettings> LoadSettingsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    // No settings file - return defaults
                    return new AppSettings();
                }

                var json = await File.ReadAllTextAsync(_settingsFilePath, cancellationToken);
                var settings = JsonSerializer.Deserialize(json, AppSettingsContext.Default.AppSettings);

                return settings ?? new AppSettings();
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled - return defaults
                return new AppSettings();
            }
            catch (Exception ex)
            {
                // Log error in production - for now just return defaults
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                return new AppSettings();
            }
        }

        public async Task<bool> SaveSettingsAsync(AppSettings settings, CancellationToken cancellationToken = default)
        {
            try
            {
                // Ensure directory still exists
                Directory.CreateDirectory(_settingsDirectory);

                var json = JsonSerializer.Serialize(settings, AppSettingsContext.Default.AppSettings);

                // Write to temp file first, then move (atomic operation)
                var tempFile = _settingsFilePath + ".tmp";
                await File.WriteAllTextAsync(tempFile, json, cancellationToken);

                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                }

                File.Move(tempFile, _settingsFilePath);

                System.Diagnostics.Debug.WriteLine($"Settings saved successfully to: {_settingsFilePath}");
                return true;
            }
            catch (OperationCanceledException)
            {
                // Operation was cancelled
                System.Diagnostics.Debug.WriteLine("Settings save cancelled");
                return false;
            }
            catch (Exception ex)
            {
                // Log error in production
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex}");
                System.Diagnostics.Debug.WriteLine($"Settings path: {_settingsFilePath}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                // Try to write error to a log file for published apps
                try
                {
                    var errorLogPath = Path.Combine(_settingsDirectory, "error.log");
                    await File.AppendAllTextAsync(errorLogPath, 
                        $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Save error: {ex}\n", cancellationToken);
                }
                catch { /* Ignore if we can't write the error log */ }

                return false;
            }
        }

        public async Task ResetSettingsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                }

                // Save default settings
                await SaveSettingsAsync(new AppSettings(), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("Settings reset cancelled");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error resetting settings: {ex.Message}");
            }
        }

        public string GetSettingsFilePath()
        {
            return _settingsFilePath;
        }
    }
}
