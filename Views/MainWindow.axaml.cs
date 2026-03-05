// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using TempConvPro.ViewModels;
using TempConvPro.Services;
using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Input;
using Avalonia;

namespace TempConvPro.Views
{
    public partial class MainWindow : Window
    {
        private readonly ISettingsService _settingsService;

        public MainWindow()
        {
            InitializeComponent();
            _settingsService = new JsonSettingsService();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Load settings
            if (DataContext is MainWindowViewModel vm)
            {
                // Inject file service
                vm.SetFileService(new AvaloniaFileService(this));

                await vm.InitializeAsync();
            }

            // Restore window state
            await RestoreWindowStateAsync();
        }

        private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            // Save window state
            await SaveWindowStateAsync();

            // Save app settings
            if (DataContext is MainWindowViewModel vm)
            {
                await vm.SaveCurrentSettingsAsync();
            }
        }

        private void About_Click(object? sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            _ = aboutWindow.ShowDialog(this);
        }

        private void Settings_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm)
            {
                var settingsWindow = new SettingsWindow
                {
                    DataContext = vm
                };
                _ = settingsWindow.ShowDialog(this);
            }
        }

        private void Formulas_Click(object? sender, RoutedEventArgs e)
        {
            var formulasWindow = new FormulasWindow();
            formulasWindow.Show(); // Non-modal - doesn't block main window
        }

        private async Task RestoreWindowStateAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();

            // Restore size
            Width = settings.WindowWidth;
            Height = settings.WindowHeight;

            // Restore position if saved
            if (settings.WindowX.HasValue && settings.WindowY.HasValue)
            {
                Position = new Avalonia.PixelPoint(
                    (int)settings.WindowX.Value,
                    (int)settings.WindowY.Value);
            }
        }

        private async Task SaveWindowStateAsync()
        {
            var settings = await _settingsService.LoadSettingsAsync();

            // Save current size and position
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            settings.WindowX = Position.X;
            settings.WindowY = Position.Y;

            await _settingsService.SaveSettingsAsync(settings);
        }
    }
}
