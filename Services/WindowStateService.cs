// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia;
using System.Threading.Tasks;
using TempConvPro.Models;

namespace TempConvPro.Services
{
    /// <summary>
    /// Service for managing window state persistence
    /// Pure MVVM - separates window state logic from View
    /// </summary>
    public interface IWindowStateService
    {
        /// <summary>
        /// Load window state from settings
        /// </summary>
        Task<WindowStateInfo> LoadWindowStateAsync();

        /// <summary>
        /// Save window state to settings
        /// </summary>
        Task SaveWindowStateAsync(WindowStateInfo state);
    }

    /// <summary>
    /// Window state data
    /// </summary>
    public class WindowStateInfo
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
    }

    /// <summary>
    /// Implementation of window state service using settings service
    /// </summary>
    public class WindowStateService : IWindowStateService
    {
        private readonly ISettingsService _settingsService;

        public WindowStateService(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public async Task<WindowStateInfo> LoadWindowStateAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();

            return new WindowStateInfo
            {
                Width = settings.WindowWidth,
                Height = settings.WindowHeight,
                X = settings.WindowX,
                Y = settings.WindowY
            };
        }

        public async Task SaveWindowStateAsync(WindowStateInfo state)
        {
            var settings = await _settingsService.LoadSettingsAsync();

            settings.WindowWidth = state.Width;
            settings.WindowHeight = state.Height;
            settings.WindowX = state.X;
            settings.WindowY = state.Y;

            await _settingsService.SaveSettingsAsync(settings);
        }
    }
}
