// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TempConvPro.Services
{
    /// <summary>
    /// JSON source generation context for export data
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(ExportData))]
    internal partial class ExportDataContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Export data model for JSON serialization
    /// </summary>
    public class ExportData
    {
        [JsonPropertyName("exportDate")]
        public DateTime ExportDate { get; set; }

        [JsonPropertyName("appName")]
        public string AppName { get; set; } = string.Empty;

        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        [JsonPropertyName("conversions")]
        public List<ConversionEntry> Conversions { get; set; } = new();
    }

    /// <summary>
    /// Individual conversion entry for export
    /// </summary>
    public class ConversionEntry
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("conversion")]
        public string Conversion { get; set; } = string.Empty;

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Service for file operations (export, import, etc.)
    /// Pure MVVM - ViewModel has no knowledge of file dialogs
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Export data to a file chosen by user
        /// </summary>
        Task<bool> ExportToFileAsync(string content, string defaultFileName, FileExportFormat format);

        /// <summary>
        /// Export collection to CSV format
        /// </summary>
        string ExportToCsv(IEnumerable<string> items, string header = "Conversion");

        /// <summary>
        /// Export collection to JSON format
        /// </summary>
        string ExportToJson(IEnumerable<string> items);

        /// <summary>
        /// Export collection to plain text
        /// </summary>
        string ExportToText(IEnumerable<string> items);
    }

    /// <summary>
    /// File export format options
    /// </summary>
    public enum FileExportFormat
    {
        Csv,
        Json,
        Text
    }

    /// <summary>
    /// Avalonia-based file service implementation
    /// </summary>
    public class AvaloniaFileService : IFileService
    {
        private readonly Window? _window;

        public AvaloniaFileService(Window? window = null)
        {
            _window = window;
        }

        public async Task<bool> ExportToFileAsync(string content, string defaultFileName, FileExportFormat format)
        {
            if (_window == null)
                return false;

            try
            {
                var storageProvider = _window.StorageProvider;
                if (storageProvider == null)
                    return false;

                // Define file type filters based on format
                var fileType = format switch
                {
                    FileExportFormat.Csv => new FilePickerFileType("CSV File")
                    {
                        Patterns = new[] { "*.csv" }
                    },
                    FileExportFormat.Json => new FilePickerFileType("JSON File")
                    {
                        Patterns = new[] { "*.json" }
                    },
                    FileExportFormat.Text => new FilePickerFileType("Text File")
                    {
                        Patterns = new[] { "*.txt" }
                    },
                    _ => new FilePickerFileType("All Files")
                    {
                        Patterns = new[] { "*.*" }
                    }
                };

                // Show save dialog
                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Export Conversion History",
                    SuggestedFileName = defaultFileName,
                    FileTypeChoices = new[] { fileType }
                });

                if (file != null)
                {
                    // Write content to file
                    await using var stream = await file.OpenWriteAsync();
                    await using var writer = new System.IO.StreamWriter(stream, Encoding.UTF8);
                    await writer.WriteAsync(content);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error exporting file: {ex.Message}");
                return false;
            }
        }

        public string ExportToCsv(IEnumerable<string> items, string header = "Conversion")
        {
            var sb = new StringBuilder();
            
            // Header
            sb.AppendLine($"{header},Timestamp");

            // Data rows
            var timestamp = DateTime.Now;
            foreach (var item in items)
            {
                // Escape quotes in CSV
                var escaped = item.Replace("\"", "\"\"");
                sb.AppendLine($"\"{escaped}\",\"{timestamp:yyyy-MM-dd HH:mm:ss}\"");
            }

            return sb.ToString();
        }

        public string ExportToJson(IEnumerable<string> items)
        {
            var timestamp = DateTime.Now;
            var data = new ExportData
            {
                ExportDate = timestamp,
                AppName = "Temperature Converter Pro",
                Version = "1.0",
                Conversions = items.Select((item, index) => new ConversionEntry
                {
                    Index = index + 1,
                    Conversion = item,
                    Timestamp = timestamp
                }).ToList()
            };

            return JsonSerializer.Serialize(data, ExportDataContext.Default.ExportData);
        }

        public string ExportToText(IEnumerable<string> items)
        {
            var sb = new StringBuilder();
            sb.AppendLine("===========================================");
            sb.AppendLine("  Temperature Converter Pro - Export");
            sb.AppendLine($"  Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("===========================================");
            sb.AppendLine();
            sb.AppendLine("Conversion History:");
            sb.AppendLine("-------------------------------------------");

            int index = 1;
            foreach (var item in items)
            {
                sb.AppendLine($"{index}. {item}");
                index++;
            }

            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"Total: {items.Count()} conversion(s)");
            sb.AppendLine();
            sb.AppendLine("Generated by Temperature Converter Pro");

            return sb.ToString();
        }
    }
}
