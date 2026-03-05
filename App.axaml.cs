// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using TempConvPro.ViewModels;
using TempConvPro.Views;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace TempConvPro
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
                // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
                DisableAvaloniaDataAnnotationValidation();

                // Initialize services (Dependency Injection setup)
                var settingsService = new Services.JsonSettingsService();
                var clipboardService = new Services.ClipboardService();
                var windowStateService = new Services.WindowStateService(settingsService);

                // Create main window with injected window state service
                var mainWindow = new MainWindow(windowStateService);

                // Create file service (needs window reference)
                var fileService = new Services.AvaloniaFileService(mainWindow);

                // Create ViewModel with all dependencies
                var viewModel = new MainWindowViewModel(clipboardService, settingsService, fileService);

                // Set DataContext and assign window
                mainWindow.DataContext = viewModel;
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", 
            Justification = "DataValidators are used by Avalonia's binding system and are preserved by the framework.")]
        private void DisableAvaloniaDataAnnotationValidation()
        {
            // Get an array of plugins to remove
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            // remove each entry found
            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}