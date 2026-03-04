// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace TempConvPro.Services
{
    /// <summary>
    /// Service for clipboard operations
    /// Allows ViewModel to interact with clipboard without knowing about the View
    /// Pure MVVM - no View dependencies in ViewModel
    /// </summary>
    public interface IClipboardService
    {
        Task SetTextAsync(string text);
    }

    public class ClipboardService : IClipboardService
    {
        public async Task SetTextAsync(string text)
        {
            // Get the main window from the application
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = desktop.MainWindow;
                if (mainWindow != null)
                {
                    var clipboard = TopLevel.GetTopLevel(mainWindow)?.Clipboard;
                    if (clipboard != null)
                    {
                        await clipboard.SetTextAsync(text);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Message to request clipboard copy operation
    /// Used for loose coupling between ViewModel and View services
    /// </summary>
    public class CopyToClipboardMessage
    {
        public string Text { get; }

        public CopyToClipboardMessage(string text)
        {
            Text = text;
        }
    }
}
