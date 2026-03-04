// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Input;
using System.Diagnostics;

namespace TempConvPro.Views
{
    public partial class FormulasWindow : Window
    {
        public FormulasWindow()
        {
            InitializeComponent();
        }

        private void OpenWikipedia(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch
            {
                // Silently fail if browser can't be opened
            }
        }

        private void Celsius_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/Celsius");
        }

        private void Fahrenheit_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/Fahrenheit");
        }

        private void Kelvin_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/Kelvin");
        }

        private void Rankine_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/Rankine_scale");
        }

        private void Reaumur_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/R%C3%A9aumur_scale");
        }

        private void Romer_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/R%C3%B8mer_scale");
        }

        private void Newton_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/Newton_scale");
        }

        private void Delisle_Click(object? sender, PointerPressedEventArgs e)
        {
            OpenWikipedia("https://en.wikipedia.org/wiki/Delisle_scale");
        }
    }
}
