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
        private readonly IWindowStateService _windowStateService;

        public MainWindow(IWindowStateService windowStateService)
        {
            InitializeComponent();
            _windowStateService = windowStateService;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
        }

        private async void MainWindow_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Initialize ViewModel
            if (DataContext is MainWindowViewModel vm)
            {
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
            var state = await _windowStateService.LoadWindowStateAsync();

            // Restore size
            Width = state.Width;
            Height = state.Height;

            // Restore position if saved
            if (state.X.HasValue && state.Y.HasValue)
            {
                Position = new Avalonia.PixelPoint(
                    (int)state.X.Value,
                    (int)state.Y.Value);
            }
        }

        private async Task SaveWindowStateAsync()
        {
            var state = new WindowStateInfo
            {
                Width = Width,
                Height = Height,
                X = Position.X,
                Y = Position.Y
            };

            await _windowStateService.SaveWindowStateAsync(state);
        }
    }
}
