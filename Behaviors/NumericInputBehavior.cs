// Copyright (c) 2026 Kevin Coleman
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;

namespace TempConvPro.Behaviors
{
    /// <summary>
    /// Attached behavior that restricts TextBox input to numeric values only
    /// Supports: integers, decimals, negative numbers
    /// Pure MVVM - declarative in XAML
    /// </summary>
    public static partial class NumericInputBehavior
    {
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "IsEnabled",
                typeof(NumericInputBehavior));

        public static readonly AttachedProperty<bool> AllowNegativeProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "AllowNegative",
                typeof(NumericInputBehavior),
                defaultValue: true);

        public static readonly AttachedProperty<bool> AllowDecimalProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>(
                "AllowDecimal",
                typeof(NumericInputBehavior),
                defaultValue: true);

        static NumericInputBehavior()
        {
            IsEnabledProperty.Changed.AddClassHandler<TextBox>(OnIsEnabledChanged);
        }

        public static bool GetIsEnabled(Control control) => control.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(Control control, bool value) => control.SetValue(IsEnabledProperty, value);

        public static bool GetAllowNegative(Control control) => control.GetValue(AllowNegativeProperty);
        public static void SetAllowNegative(Control control, bool value) => control.SetValue(AllowNegativeProperty, value);

        public static bool GetAllowDecimal(Control control) => control.GetValue(AllowDecimalProperty);
        public static void SetAllowDecimal(Control control, bool value) => control.SetValue(AllowDecimalProperty, value);

        private static void OnIsEnabledChanged(TextBox textBox, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool isEnabled)
            {
                if (isEnabled)
                {
                    textBox.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
                }
                else
                {
                    textBox.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
                }
            }
        }

        private static void OnTextInput(object? sender, TextInputEventArgs e)
        {
            if (sender is not TextBox textBox || string.IsNullOrEmpty(e.Text))
                return;

            var allowNegative = GetAllowNegative(textBox);
            var allowDecimal = GetAllowDecimal(textBox);

            // Get current text and selection
            var currentText = textBox.Text ?? string.Empty;
            var selectionStart = textBox.SelectionStart;
            var selectionEnd = textBox.SelectionEnd;

            // Build what the text would be after this input
            // Guard against invalid selection range (can occur during property updates)
            var count = Math.Max(0, selectionEnd - selectionStart);
            var newText = currentText.Remove(selectionStart, count);
            newText = newText.Insert(selectionStart, e.Text ?? string.Empty);

            // Validate the proposed text
            if (!IsValidNumericInput(newText, allowNegative, allowDecimal))
            {
                e.Handled = true;
            }
        }

        private static bool IsValidNumericInput(string text, bool allowNegative, bool allowDecimal)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            // Build regex pattern based on options
            var pattern = "^";

            if (allowNegative)
                pattern += "-?"; // Optional negative sign

            pattern += @"\d*"; // Digits

            if (allowDecimal)
                pattern += @"\.?\d*"; // Optional decimal point and more digits

            pattern += "$";

            return Regex.IsMatch(text, pattern);
        }
    }
}
