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
using Microsoft.Extensions.DependencyInjection;
using TempConvPro.Services;

namespace TempConvPro
{
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

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

                // Create main window first (needed for FileService registration)
                var windowStateService = new WindowStateService(new JsonSettingsService());
                var mainWindow = new MainWindow(windowStateService);

                // Configure dependency injection container
                _serviceProvider = ServiceConfiguration.ConfigureServices(mainWindow);

                // Resolve ViewModel from DI container
                var viewModel = _serviceProvider.GetRequiredService<MainWindowViewModel>();

                // Set DataContext and assign window
                mainWindow.DataContext = viewModel;
                desktop.MainWindow = mainWindow;

                // Handle application shutdown to dispose services
                desktop.ShutdownRequested += (s, e) =>
                {
                    _serviceProvider?.Dispose();
                };
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