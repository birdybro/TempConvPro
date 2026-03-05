// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TempConvPro.Services;
using TempConvPro.ViewModels;
using TempConvPro.Views;

namespace TempConvPro
{
    /// <summary>
    /// Configures dependency injection for the application
    /// Centralizes all service registrations for maintainability
    /// </summary>
    public static class ServiceConfiguration
    {
        /// <summary>
        /// Configure and build the service provider with all application dependencies
        /// </summary>
        public static ServiceProvider ConfigureServices(Window mainWindow)
        {
            var services = new ServiceCollection();

            // Register Services (Singletons - shared across app lifetime)
            services.AddSingleton<ISettingsService, JsonSettingsService>();
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<IWindowStateService, WindowStateService>();

            // Register FileService with the main window reference
            services.AddSingleton<IFileService>(sp => new AvaloniaFileService(mainWindow));

            // Register ViewModels (Transient - new instance each time)
            services.AddTransient<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }
    }
}
