// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TempConvPro.Views
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            // Wire up close button
            var closeButton = this.FindControl<Button>("CloseButton");
            if (closeButton != null)
            {
                closeButton.Click += (s, e) => Close();
            }
        }
    }
}
