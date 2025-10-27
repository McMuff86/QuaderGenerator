# QuaderGenerator for Rhino

[![Rhino Version](https://img.shields.io/badge/Rhino-7%20%7C%208-blue)](https://www.rhino3d.com/)
[![.NET](https://img.shields.io/badge/.NET-7.0%20%7C%20Framework%204.8-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

A powerful Rhino plugin for generating box (Quader) geometry with advanced calculation modes, preset management, and object analysis tools.

![QuaderGenerator Panel](docs/images/panel-screenshot.png)

---

## 🚀 Features

### 🎯 Three Generation Modes
- **Direct Dimensions** - Specify length, width, and height directly
- **From Volume** - Calculate dimensions from target volume
- **From Surface Area** - Calculate dimensions from target surface area

### 📏 Smart Unit System
- **Multiple Units:** mm, cm, m
- **Automatic Conversion:** All values converted to Rhino's internal mm
- **Dynamic Labels:** UI adapts to selected unit

### 💾 Preset Management
- **Save & Load:** Store frequently used configurations
- **7 Default Presets:** Including 1m³ cube, 7m³ cube, surface area boxes, and more
- **Unit-Aware:** Each preset remembers its unit

### 🔍 Object Analysis
- **Real-time Info:** Analyze any selected object in Rhino
- **Comprehensive Data:**
  - Volume (for solids)
  - Surface Area
  - Curve length and properties
  - Mesh statistics
  - Layer information

---

## 📦 Installation

### Method 1: Package Manager (Recommended)
```
1. Open Rhino
2. Type: _PackageManager
3. Search: "QuaderGenerator"
4. Click: Install
```

### Method 2: Manual Installation
1. Download the latest `.yak` file from [Releases](https://github.com/yourusername/QuaderGenerator/releases)
2. In Rhino, type `_PackageManager`
3. Click "Install from File"
4. Select the downloaded `.yak` file

### Method 3: From Source
```bash
git clone https://github.com/yourusername/QuaderGenerator.git
cd QuaderGenerator
dotnet build QuaderGenerator.csproj --configuration Release
```

---

## 🎮 Quick Start

### 1. Open the Panel
```
QuaderGen
```

### 2. Generate a Simple Box
1. Select **Units**: `m`
2. Select **Mode**: `Direct Dimensions`
3. Enter: `1` × `1` × `1`
4. Click: `Generate Quader`

Result: 1m × 1m × 1m cube at origin ✓

### 3. Use a Preset
1. Select from dropdown: `Cube 1m³`
2. Click: `Generate Quader`

### 4. Analyze an Object
1. Select any object in Rhino
2. Click: `Refresh Info` in panel
3. View: Surface area, volume, and more!

---

## 📖 Documentation

- **[Usage Guide](Usage.md)** - Complete user manual with examples
- **[Agents.md](Agents.md)** - For AI agents and developers
- **[API Documentation](docs/api.md)** - Code reference *(coming soon)*
- **[Contributing](CONTRIBUTING.md)** - How to contribute *(coming soon)*

---

## 🛠️ Technical Details

### Built With
- **RhinoCommon** 8.0+ (Rhino SDK)
- **Eto.Forms** 2.8.3 (Cross-platform UI)
- **.NET 7** / **.NET Framework 4.8**
- **System.Text.Json** 8.0.5 (Preset serialization)

### System Requirements
- **Rhino 7** or **Rhino 8**
- **Windows** 10/11 or **macOS** 10.15+
- **.NET 7 Runtime** (for Rhino 8) or **.NET Framework 4.8** (for Rhino 7)

### Project Structure
```
QuaderGenerator/
├── Commands/
│   └── QuaderGenCommand.cs       # Command registration
├── UI/
│   └── QuaderPanel.cs             # Main UI panel
├── Core/
│   ├── QuaderCalculator.cs        # Math calculations
│   ├── QuaderPreset.cs            # Preset data model
│   ├── PresetManager.cs           # Save/load logic
│   └── UnitConverter.cs           # Unit conversions
├── QuaderGeneratorPlugin.cs       # Plugin entry point
└── Agents.md                       # Developer documentation
```

---

## 🎯 Use Cases

### Architecture
- Quick box prototyping
- Volume calculations for room sizing
- Surface area estimates for material quantities

### Engineering
- Generate test geometries with precise volumes
- Calculate dimensions from specifications
- Analyze existing models

### Education
- Teach volume/surface area relationships
- Demonstrate unit conversions
- Practice 3D modeling basics

---

## 📊 Example Calculations

### Cube from Volume
**Input:**
- Volume: 7 m³
- Length: 1.913 m
- Width: 1.913 m

**Result:**
- Height: **1.913 m** (calculated)
- Actual Volume: **7.000 m³** ✓

### Box from Surface Area
**Input:**
- Surface Area: 100 m²
- Length: 5 m
- Width: 5 m

**Result:**
- Height: **3.333 m** (calculated)
- Actual Surface: **100.000 m²** ✓

---

## 🤝 Contributing

Contributions are welcome! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details.

### Development Setup
1. Clone the repository
2. Open `QuaderGenerator.sln` in Visual Studio 2022
3. Install Rhino Visual Studio Extension
4. Press F5 to debug (Rhino launches automatically)

### Testing
- Unit tests: `dotnet test` *(coming soon)*
- Manual testing: See [Agents.md](Agents.md) for test scenarios

---

## 📝 Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history.

### Latest Version: 1.0.0
- ✅ Three generation modes
- ✅ mm/cm/m unit support
- ✅ Preset management
- ✅ Object analysis tool
- ✅ Cross-platform (Windows/Mac)

---

## 📄 License

This project is licensed under the MIT License - see [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- [McNeel & Associates](https://www.mcneel.com/) for Rhino and RhinoCommon
- [Eto.Forms](https://github.com/picoe/Eto) for cross-platform UI framework
- Community contributors and testers

---

## 📞 Support

- **Issues:** [GitHub Issues](https://github.com/yourusername/QuaderGenerator/issues)
- **Discussions:** [Rhino Forum](https://discourse.mcneel.com/)
- **Email:** your.email@example.com

---

## 🌟 Star History

If you find this plugin useful, please consider giving it a star ⭐

[![Star History Chart](https://api.star-history.com/svg?repos=yourusername/QuaderGenerator&type=Date)](https://star-history.com/#yourusername/QuaderGenerator&Date)

---

**Made with ❤️ for the Rhino community**

