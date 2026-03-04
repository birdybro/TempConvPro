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
            // Create a simple About window in code
            var aboutWindow = new Window
            {
                Title = "About Temperature Converter Pro",
                Width = 450,
                Height = 500,
                CanResize = false,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var closeButton = new Button
            {
                Content = "Close",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Padding = new Thickness(40, 10)
            };
            closeButton.Click += (s, args) => aboutWindow.Close();

            aboutWindow.Content = new ScrollViewer
            {
                Content = new StackPanel
                {
                    Margin = new Thickness(20),
                    Children =
                    {
                        // Title
                        new TextBlock
                        {
                            Text = "🌡️",
                            FontSize = 48,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 10)
                        },
                        new TextBlock
                        {
                            Text = "Temperature Converter Pro",
                            FontSize = 24,
                            FontWeight = Avalonia.Media.FontWeight.Bold,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
                        },
                        new TextBlock
                        {
                            Text = "Version 1.0.0",
                            FontSize = 14,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(0, 5, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = "© 2026 Kevin Coleman",
                            FontSize = 12,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 20)
                        },

                        // Description
                        new TextBlock
                        {
                            Text = "A comprehensive temperature conversion tool supporting 8 temperature scales (4 modern + 4 historical) with advanced features.",
                            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                            Margin = new Thickness(0, 0, 0, 15)
                        },

                        // Features
                        new TextBlock
                        {
                            Text = "✨ Features",
                            FontWeight = Avalonia.Media.FontWeight.Bold,
                            FontSize = 14,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = "• 4 Modern Scales: Celsius, Fahrenheit, Kelvin, Rankine\n" +
                                   "• 4 Historical Scales: Réaumur, Rømer, Newton, Delisle\n" +
                                   "• Keyboard Shortcuts (Ctrl+C/R/H/S)\n" +
                                   "• Custom Temperature Presets\n" +
                                   "• Formula Display Panel\n" +
                                   "• Export to CSV/JSON/TXT\n" +
                                   "• Settings Persistence\n" +
                                   "• Dark Mode Support",
                            FontFamily = "Consolas,Courier New,monospace",
                            FontSize = 11,
                            Margin = new Thickness(0, 0, 0, 15)
                        },

                        // Built With
                        new TextBlock
                        {
                            Text = "🛠️ Technology Stack",
                            FontWeight = Avalonia.Media.FontWeight.Bold,
                            FontSize = 14,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = "• .NET 10\n" +
                                   "• Avalonia UI 11.x\n" +
                                   "• CommunityToolkit.Mvvm 8.x\n" +
                                   "• Pure MVVM Architecture",
                            FontFamily = "Consolas,Courier New,monospace",
                            FontSize = 11,
                            Margin = new Thickness(0, 0, 0, 15)
                        },

                        // License
                        new TextBlock
                        {
                            Text = "📄 License",
                            FontWeight = Avalonia.Media.FontWeight.Bold,
                            FontSize = 14,
                            Margin = new Thickness(0, 0, 0, 5)
                        },
                        new TextBlock
                        {
                            Text = "MIT License - See LICENSE file for details.",
                            FontSize = 11,
                            Margin = new Thickness(0, 0, 0, 20)
                        },

                        // Close button
                        closeButton
                    }
                }
            };

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
