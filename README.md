# Temperature Converter Pro 🌡️

A professional temperature conversion desktop application built with **.NET 10** and **Avalonia UI**, featuring 8 temperature scales with pure MVVM architecture.

## Features

### 🌡️ Temperature Scales
- **Modern:** Celsius, Fahrenheit, Kelvin, Rankine
- **Historical:** Réaumur, Rømer, Newton, Delisle (18th-century scales)
- **Toggle:** Show/hide historical scales
- **Real-time:** All scales update as you type

### ✨ Core Features
- **Custom Presets** - Save frequently used temperatures
- **Formula Display** - See conversion math
- **Export** - Save history to CSV, JSON, or TXT
- **Conversion History** - Last 10 conversions tracked
- **Absolute Zero Warning** - Physics-aware validation
- **Settings Persistence** - Preferences auto-saved

### 🎨 User Experience
- **Keyboard Shortcuts** - Ctrl+C (copy), Ctrl+R (reset), Ctrl+H (clear history), Ctrl+S (settings)
- **Context Menus** - Right-click for quick actions
- **Input Validation** - Numeric-only fields
- **Fixed Precision** - 2 decimal places (perfect balance)
- **Dark Mode** - Automatic theme support
- **Dynamic Window** - Automatically sizes to content

### 🛡️ Professional Quality
- **Error Handling** - Graceful degradation, never crashes
- **About Dialog** - Version info and feature showcase
- **Status Feedback** - Clear user notifications

## Architecture

**Pattern:** Pure MVVM with Dependency Injection  
**Framework:** Avalonia UI 11.x  
**Toolkit:** CommunityToolkit.Mvvm 8.x  
**Target:** .NET 10  

**Structure:**
```
TempConvPro/
├── Views/              # XAML UI + minimal code-behind
├── ViewModels/         # Business logic + data binding
├── Models/             # Data models (AppSettings, TemperaturePreset)
├── Services/           # File I/O, clipboard, settings persistence
├── Behaviors/          # Reusable UI behaviors (numeric input validation)
└── Assets/             # Icons and resources
```

**Key Patterns:**
- ObservableProperty source generators
- RelayCommand for actions
- Interface-based services for testability
- Culture-invariant number parsing

## Conversion Formulas

**Celsius (C) as Base:**
```
Fahrenheit:  F = C × (9/5) + 32
Kelvin:      K = C + 273.15
Rankine:     R = (C + 273.15) × (9/5)
Réaumur:     Ré = C × (4/5)
Rømer:       Rø = C × (21/40) + 7.5
Newton:      N = C × (33/100)
Delisle:     De = (100 - C) × (3/2)    [inverted scale]
```

**Key Temperatures:**
- Absolute Zero: -273.15°C = 0K = 0°R
- Water Freezes: 0°C = 32°F = 273.15K
- Water Boils: 100°C = 212°F = 373.15K
- Body Temp: 37°C = 98.6°F = 310.15K

## Quick Start

**Prerequisites:** .NET 10 SDK

**Build:**
```bash
dotnet build
```

**Run:**
```bash
dotnet run
```

**Debug:** Open `TempConvPro.slnx` in Visual Studio 2025+ or Rider and press F5

## Usage

**Convert:** Type in any temperature field, all others update instantly  
**Presets:** Select from dropdown or save custom favorites  
**Export:** Click CSV/JSON/TXT buttons to save history  
**Formulas:** Toggle panel to see conversion mathematics  
**Copy:** Ctrl+C or click "Copy Result" button  
**Reset:** Ctrl+R or click "Clear All"  

## License

MIT License - Copyright (c) 2026 Kevin Coleman

See [LICENSE](LICENSE) file for full license text.
