using System.Text.Json.Serialization;

// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempConvPro.Models
{
    /// <summary>
    /// JSON source generation context for trim-safe serialization
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(AppSettings))]
    internal partial class AppSettingsContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Application settings that persist between sessions
    /// Serialized to JSON and saved to local storage
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Last entered Celsius value
        /// </summary>
        [JsonPropertyName("lastCelsius")]
        public string LastCelsius { get; set; } = "0";

        /// <summary>
        /// Last entered Fahrenheit value
        /// </summary>
        [JsonPropertyName("lastFahrenheit")]
        public string LastFahrenheit { get; set; } = "32";

        /// <summary>
        /// Last entered Kelvin value
        /// </summary>
        [JsonPropertyName("lastKelvin")]
        public string LastKelvin { get; set; } = "273.15";

        /// <summary>
        /// Window width
        /// </summary>
        [JsonPropertyName("windowWidth")]
        public double WindowWidth { get; set; } = 550;

        /// <summary>
        /// Window height
        /// </summary>
        [JsonPropertyName("windowHeight")]
        public double WindowHeight { get; set; } = 950;

        /// <summary>
        /// Window X position (null = centered)
        /// </summary>
        [JsonPropertyName("windowX")]
        public double? WindowX { get; set; }

        /// <summary>
        /// Window Y position (null = centered)
        /// </summary>
        [JsonPropertyName("windowY")]
        public double? WindowY { get; set; }

        /// <summary>
        /// Whether to show conversion history on startup
        /// </summary>
        [JsonPropertyName("showHistory")]
        public bool ShowHistory { get; set; } = true;

        /// <summary>
        /// Maximum number of history entries to keep
        /// </summary>
        [JsonPropertyName("maxHistoryEntries")]
        public int MaxHistoryEntries { get; set; } = 10;

        /// <summary>
        /// Auto-save settings on every change
        /// </summary>
        [JsonPropertyName("autoSave")]
        public bool AutoSave { get; set; } = true;

        /// <summary>
        /// Restore last values on startup
        /// </summary>
        [JsonPropertyName("restoreLastValues")]
        public bool RestoreLastValues { get; set; } = true;

        /// <summary>
        /// Number of decimal places to display (0-8)
        /// </summary>
        [JsonPropertyName("decimalPlaces")]
        public int DecimalPlaces { get; set; } = 2;
    }
}
