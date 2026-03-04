// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Text.Json;
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
        Task<AppSettings> LoadSettingsAsync();

        /// <summary>
        /// Save settings to storage
        /// </summary>
        Task<bool> SaveSettingsAsync(AppSettings settings);

        /// <summary>
        /// Reset to default settings
        /// </summary>
        Task ResetSettingsAsync();

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

        public async Task<AppSettings> LoadSettingsAsync()
        {
            try
            {
                if (!File.Exists(_settingsFilePath))
                {
                    // No settings file - return defaults
                    return new AppSettings();
                }

                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json);
                
                return settings ?? new AppSettings();
            }
            catch (Exception ex)
            {
                // Log error in production - for now just return defaults
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
                return new AppSettings();
            }
        }

        public async Task<bool> SaveSettingsAsync(AppSettings settings)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true // Pretty print for readability
                };

                var json = JsonSerializer.Serialize(settings, options);
                await File.WriteAllTextAsync(_settingsFilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                // Log error in production
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
                return false;
            }
        }

        public async Task ResetSettingsAsync()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    File.Delete(_settingsFilePath);
                }

                // Save default settings
                await SaveSettingsAsync(new AppSettings());
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
