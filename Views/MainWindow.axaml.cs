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
        private HistoricalScalesWindow? _historicalScalesWindow;

        /// <summary>
        /// Parameterless constructor for XAML designer and runtime loader
        /// </summary>
        public MainWindow() : this(new WindowStateService(new JsonSettingsService()))
        {
        }

        /// <summary>
        /// Constructor with dependency injection for production use
        /// </summary>
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

                // Subscribe to ShowHistoricalScales property changes
                vm.PropertyChanged += ViewModel_PropertyChanged;
            }

            // Restore window state
            await RestoreWindowStateAsync();
        }

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainWindowViewModel.ShowHistoricalScales))
            {
                if (DataContext is MainWindowViewModel vm)
                {
                    HandleHistoricalScalesWindow(vm.ShowHistoricalScales);
                }
            }
        }

        private void HandleHistoricalScalesWindow(bool show)
        {
            if (show)
            {
                // Open the historical scales window if not already open
                if (_historicalScalesWindow == null || !_historicalScalesWindow.IsVisible)
                {
                    _historicalScalesWindow = new HistoricalScalesWindow
                    {
                        DataContext = this.DataContext // Share the same ViewModel
                    };

                    // Position window to the right of main window
                    _historicalScalesWindow.Position = new PixelPoint(
                        Position.X + (int)Width + 10,
                        Position.Y
                    );

                    // Handle window closing
                    _historicalScalesWindow.Closed += (s, e) =>
                    {
                        _historicalScalesWindow = null;

                        // Uncheck the checkbox when window is closed
                        if (DataContext is MainWindowViewModel vm)
                        {
                            vm.ShowHistoricalScales = false;
                        }
                    };

                    _historicalScalesWindow.Show();
                }
            }
            else
            {
                // Close the historical scales window
                _historicalScalesWindow?.Close();
                _historicalScalesWindow = null;
            }
        }

        private async void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            // Close historical scales window if open
            _historicalScalesWindow?.Close();

            // Save window state
            await SaveWindowStateAsync();

            // Save app settings and dispose ViewModel
            if (DataContext is MainWindowViewModel vm)
            {
                vm.PropertyChanged -= ViewModel_PropertyChanged;
                await vm.SaveCurrentSettingsAsync();
                await vm.DisposeAsync();
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
