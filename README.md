# Temperature Converter Pro 🌡️

A professional-grade temperature conversion desktop application built with **.NET 10** and **Avalonia UI**, featuring 9 temperature scales, advanced history management, and a polished cross-platform UI with pure MVVM architecture.

---

## ✨ Features

### 🌡️ Temperature Scales
- **Modern Standards:** Celsius (°C), Fahrenheit (°F), Kelvin (K), Rankine (°R)
- **Historical Scales:** Réaumur (°Ré), Rømer (°Rø), Newton (°N), Delisle (°De)
- **Collapsible Panel:** Toggle historical scales visibility for a cleaner interface
- **Real-Time Conversion:** All scales update instantly as you type in any field
- **Smart Validation:** Prevents negative Kelvin/Rankine values (absolute zero enforcement)
- **Absolute Zero Warning:** Physics-aware alerts when approaching or exceeding physical limits

### 📊 Conversion History
- **Automatic Tracking:** Records all temperature conversions with timestamps
- **Multiple Export Formats:** Export history to **CSV**, **JSON**, or **TXT** files
- **Quick Clear:** Clear history with Ctrl+H or dedicated button
- **Scrollable View:** Browse through conversion history with smooth scrolling
- **Persistent Storage:** History survives app restarts (when enabled in settings)

### ⭐ Presets & Shortcuts
- **Quick Presets:** Pre-configured common temperatures (water freezing/boiling, body temperature, absolute zero, etc.)
- **Custom Presets:** Save your frequently used temperatures with custom names
- **Keyboard Shortcuts:**
  - `Ctrl+C` - Copy current conversion to clipboard
  - `Ctrl+R` - Clear all temperature fields
  - `Ctrl+H` - Clear conversion history
- **Context Menus:** Right-click any temperature field for quick actions (copy, clear, save as preset)

### ⚙️ Settings & Customization
- **Decimal Precision:** Adjust display precision (0-8 decimal places)
- **History Management:** Configure history size limits and auto-save behavior
- **Theme Support:** Automatic dark/light mode following system preferences
- **Window State Persistence:** Remembers window position and size between sessions
- **Settings Auto-Save:** All preferences automatically saved to disk

### 🧮 Formulas & Education
- **Formula Window:** Displays all conversion formulas with detailed explanations
- **Tooltips:** Hover over any temperature scale for quick formula reference
- **Educational Context:** Learn about historical temperature scales and their origins

### 🎨 User Experience
- **Clean Modern UI:** Fluent Design System with rounded corners and smooth transitions
- **Input Validation:** Numeric-only fields with decimal and negative number support
- **Visual Feedback:** Status bar notifications for copy, save, and error actions
- **Responsive Layout:** Automatically sizes to content with min/max constraints
- **Cross-Platform:** Runs on Windows, macOS, and Linux

### 🛡️ Professional Quality
- **Robust Error Handling:** Graceful degradation, never crashes on invalid input
- **Dependency Injection:** Clean service architecture for maintainability
- **Type-Safe Bindings:** Compiled XAML bindings for performance and reliability
- **Culture-Invariant Parsing:** Consistent number formatting across locales
- **Memory Efficient:** Optimized with AOT compilation and trimming support

---

## 🏗️ Architecture

### Tech Stack
- **Framework:** .NET 10
- **UI Library:** Avalonia UI 11.3.12
- **MVVM Toolkit:** CommunityToolkit.Mvvm 8.2.1
- **Dependency Injection:** Microsoft.Extensions.DependencyInjection 10.0.3
- **Language:** C# 13 with nullable reference types enabled

### Project Structure
```
TempConvPro/
├── 📁 Views/              # XAML UI definitions + code-behind
│   ├── MainWindow.axaml           # Main converter interface
│   ├── SettingsWindow.axaml       # User preferences dialog
│   ├── FormulasWindow.axaml       # Conversion formula reference
│   └── AboutWindow.axaml          # Application info dialog
├── 📁 ViewModels/         # Business logic & data binding
│   ├── MainWindowViewModel.cs     # Core conversion logic
│   └── ViewModelBase.cs           # Shared base class
├── 📁 Models/             # Data structures
│   ├── TemperaturePreset.cs       # Preset definition
│   └── AppSettings.cs             # User preferences model
├── 📁 Services/           # Application services
│   ├── FileService.cs             # File I/O operations
│   ├── ClipboardService.cs        # Clipboard integration
│   ├── SettingsService.cs         # Settings persistence
│   └── WindowStateService.cs      # Window position/size management
├── 📁 Behaviors/          # Reusable UI behaviors
│   └── NumericInputBehavior.cs    # Numeric input validation
├── 📁 Assets/             # Icons and resources
│   └── logo.ico                   # Application icon
├── ServiceConfiguration.cs        # DI container setup
├── ViewLocator.cs                 # MVVM view resolution
├── Program.cs                     # Application entry point
└── App.axaml.cs                   # Application lifecycle
```

### Design Patterns
- **MVVM (Model-View-ViewModel):** Complete separation of UI and business logic
- **Dependency Injection:** Interface-based service registration for testability
- **Observable Properties:** Source generators for INotifyPropertyChanged
- **Relay Commands:** Type-safe command binding with CanExecute support
- **Service Locator:** Centralized service configuration and resolution
- **Repository Pattern:** Settings and file services abstract data persistence

### Key Technologies
- **Compiled Bindings:** Type-safe XAML bindings with compile-time validation
- **Async/Await:** File operations and clipboard access use async patterns
- **Culture-Invariant Parsing:** Consistent number formatting across all locales
- **ObservableCollection:** Reactive data binding for history and presets
- **Self-Contained Deployment:** Single-file executables with native libraries embedded

---

## 🧮 Conversion Formulas

All conversions use **Celsius as the base unit** for consistency and accuracy.

### From Celsius (C):
```
Fahrenheit (°F):   F = C × (9/5) + 32
Kelvin (K):        K = C + 273.15
Rankine (°R):      R = (C + 273.15) × (9/5)
Réaumur (°Ré):     Ré = C × (4/5)
Rømer (°Rø):       Rø = C × (21/40) + 7.5
Newton (°N):       N = C × (33/100)
Delisle (°De):     De = (100 - C) × (3/2)    ⚠️ Inverted scale!
```

### To Celsius (C):
```
From Fahrenheit:   C = (F - 32) × (5/9)
From Kelvin:       C = K - 273.15
From Rankine:      C = (R × 5/9) - 273.15
From Réaumur:      C = Ré × (5/4)
From Rømer:        C = (Rø - 7.5) × (40/21)
From Newton:       C = N × (100/33)
From Delisle:      C = 100 - (De × 2/3)
```

### Important Reference Points
| Description          | °C       | °F       | K       | °R      |
|---------------------|----------|----------|---------|---------|
| Absolute Zero       | -273.15  | -459.67  | 0       | 0       |
| Water Freezes       | 0        | 32       | 273.15  | 491.67  |
| Human Body Temp     | 37       | 98.6     | 310.15  | 558.27  |
| Water Boils         | 100      | 212      | 373.15  | 671.67  |

**Note:** Delisle is an **inverted scale** where higher values represent colder temperatures!

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or later)
- Windows 10/11, macOS 10.15+, or Linux with X11/Wayland

### Clone the Repository
```bash
git clone https://github.com/birdybro/TempConvPro.git
cd TempConvPro
```

### Build from Source
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run in Debug mode
dotnet run
```

### Development Environment
**Visual Studio 2025+:**
1. Open `TempConvPro.slnx` solution file
2. Press `F5` to build and run with debugging

**JetBrains Rider:**
1. Open project folder
2. Select `TempConvPro.csproj` when prompted
3. Press `F5` or click Run

**VS Code:**
1. Open project folder
2. Install "C# Dev Kit" extension
3. Press `F5` to run

### Publish Single-File Executable
```bash
# Windows (x64)
dotnet publish -c Release -r win-x64

# macOS (ARM64)
dotnet publish -c Release -r osx-arm64

# Linux (x64)
dotnet publish -c Release -r linux-x64
```

Output will be a **single self-contained executable** in:
```
bin/Release/net10.0/{runtime}/publish/
```

---

## 📖 Usage Guide

### Basic Conversion
1. **Type** a temperature value in any field (e.g., "100" in Celsius)
2. **All other scales update instantly** with converted values
3. **Copy** the result with `Ctrl+C` or the "📋 Copy Result" button

### Using Presets
1. Click the **"Quick Presets"** dropdown
2. Select a preset (e.g., "Water Boils - 100°C")
3. All temperature fields populate automatically
4. Save custom presets with the **"⭐ Save Preset"** button

### Viewing Formulas
1. Click the **"🧮 Formulas"** button
2. Browse conversion formulas for all scales
3. Learn about historical scales and their origins

### Exporting History
1. Perform multiple conversions (builds history automatically)
2. Click **CSV**, **JSON**, or **TXT** export buttons
3. Choose save location in file dialog
4. Open exported file in your preferred application

### Settings Configuration
1. Click **"⚙️ Settings"** button
2. Adjust decimal precision (0-8 places)
3. Configure history size and persistence
4. Changes save automatically

### Keyboard Shortcuts
| Shortcut | Action                      |
|----------|-----------------------------|
| `Ctrl+C` | Copy current conversion     |
| `Ctrl+R` | Clear all temperature fields|
| `Ctrl+H` | Clear conversion history    |

---
---

## 🛠️ Build Configuration

The project includes optimizations for production deployment:

### Release Build Features
- **Self-Contained:** No .NET runtime installation required
- **Single-File:** All dependencies bundled into one executable
- **Trimmed:** Unused code removed for smaller file size (~15-25 MB)
- **AOT-Ready:** IL trimming with native library support
- **Compressed:** Built-in compression for smaller downloads

### Optimization Flags (TempConvPro.csproj)
```xml
<SelfContained>true</SelfContained>
<PublishSingleFile>true</PublishSingleFile>
<PublishTrimmed>true</PublishTrimmed>
<TrimMode>link</TrimMode>
<EnableCompressionInSingleFile>true</EnableCompressionInSingleFile>
```

---

## 🤝 Contributing

Contributions are welcome! Here's how to get started:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines
- Follow existing code style and MVVM patterns
- Add XML documentation comments for public APIs
- Test on multiple platforms (Windows/macOS/Linux) when possible
- Update README if adding new features

---

## 📝 License

**MIT License** - Copyright (c) 2026 Kevin Coleman

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

**THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.**

See [LICENSE](LICENSE) file for full license text.

---

## 📞 Support & Contact

- **Issues:** [GitHub Issues](https://github.com/birdybro/TempConvPro/issues)
- **Discussions:** [GitHub Discussions](https://github.com/birdybro/TempConvPro/discussions)
- **Repository:** [https://github.com/birdybro/TempConvPro](https://github.com/birdybro/TempConvPro)

---

## 🎯 Roadmap

Potential future enhancements:
- [ ] Unit tests for conversion accuracy
- [ ] Localization/internationalization support
- [ ] Plugin system for custom temperature scales
- [ ] Graph visualization of temperature comparisons
- [ ] Command-line interface (CLI) mode
- [ ] Import presets from file
- [ ] Batch conversion from CSV input

---

## 🙏 Acknowledgments

- **Avalonia UI Team** - For the excellent cross-platform UI framework
- **.NET Community Toolkit** - For MVVM source generators
- **Microsoft** - For .NET 10 and Visual Studio
- **Historical Thermometry** - Research on 18th-century temperature scales

---

<div align="center">

**Made with ❤️ using .NET 10 and Avalonia UI**

⭐ Star this repo if you find it useful!

</div>
