// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempConvPro.Models
{
    /// <summary>
    /// Represents a preset temperature value with all scale conversions
    /// Used for quick selection of common temperatures
    /// </summary>
    public class TemperaturePreset
    {
        /// <summary>
        /// Display name of the preset (e.g., "Water Freezing")
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Icon/emoji for visual identification
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Celsius value
        /// </summary>
        public double Celsius { get; set; }

        /// <summary>
        /// Fahrenheit value (calculated using shared constants)
        /// </summary>
        public double Fahrenheit => Celsius * TemperatureConstants.CelsiusToFahrenheitFactor + TemperatureConstants.FahrenheitOffset;

        /// <summary>
        /// Kelvin value (calculated using shared constants)
        /// </summary>
        public double Kelvin => Celsius + TemperatureConstants.CelsiusToKelvinOffset;

        /// <summary>
        /// Rankine value (calculated using shared constants)
        /// </summary>
        public double Rankine => (Celsius + TemperatureConstants.CelsiusToKelvinOffset) * TemperatureConstants.CelsiusToFahrenheitFactor;

        /// <summary>
        /// Optional description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Display text for ComboBox
        /// </summary>
        public string DisplayText => $"{Icon} {Name}";

        /// <summary>
        /// Whether this is a custom user-created preset
        /// </summary>
        public bool IsCustom { get; set; } = false;

        public TemperaturePreset()
        {
        }

        public TemperaturePreset(string name, string icon, double celsius, string description = "")
        {
            Name = name;
            Icon = icon;
            Celsius = celsius;
            Description = description;
        }

        public override string ToString() => DisplayText;
    }
}
