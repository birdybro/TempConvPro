// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace TempConvPro.Models
{
    /// <summary>
    /// Shared temperature conversion constants used throughout the application.
    /// Centralizes all conversion factors to ensure consistency and maintainability.
    /// </summary>
    public static class TemperatureConstants
    {
        // Absolute zero
        public const double AbsoluteZeroCelsius = -273.15;

        // Kelvin conversions
        public const double CelsiusToKelvinOffset = 273.15;

        // Fahrenheit conversions
        public const double CelsiusToFahrenheitFactor = 9.0 / 5.0;
        public const double FahrenheitOffset = 32.0;
        public const double FahrenheitToRankineOffset = 459.67;

        // Historical scale conversions
        public const double CelsiusToReaumurFactor = 4.0 / 5.0;
        public const double CelsiusToRomerFactor = 21.0 / 40.0;
        public const double RomerOffset = 7.5;
        public const double CelsiusToNewtonFactor = 33.0 / 100.0;
        public const double DelisleBase = 100.0;
        public const double CelsiusToDelisleFactor = 3.0 / 2.0;
    }
}
