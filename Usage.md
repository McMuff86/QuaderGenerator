# QuaderGenerator - Usage Guide

**Version:** 1.0.0  
**Platform:** Rhino 8 (Windows/Mac), Rhino 7  
**Last Updated:** October 2025

## Overview

QuaderGenerator is a Rhino plugin that allows you to create box (Quader) geometry using three different methods: direct dimensions, volume-based calculation, or surface area-based calculation. It includes a preset system to save and load frequently used configurations.

---

## Installation

### Method 1: Via Yak Package Manager (Recommended)
1. Open Rhino
2. Type `_PackageManager` in the command line
3. Search for "QuaderGenerator"
4. Click "Install"

### Method 2: Manual Installation
1. Download the `.yak` file from the releases
2. In Rhino, type `_PackageManager`
3. Click "Install from File"
4. Select the downloaded `.yak` file

### Method 3: Development Mode
1. Build the project in Visual Studio (F5)
2. Rhino launches automatically with the plugin loaded

---

## Getting Started

### Opening the Panel

Type `QuaderGen` in the Rhino command line. The Quader Generator panel will open on the right side of the viewport.

---

## Generation Modes

### 1. Direct Dimensions

Create a box by specifying all three dimensions directly.

**Steps:**
1. Select "Direct Dimensions" mode
2. Enter **Length (X)** value
3. Enter **Width (Y)** value
4. Enter **Height (Z)** value
5. Click "Generate Quader"

**Example:**
- Length: 10
- Width: 5
- Height: 3
- Result: 10×5×3 box at origin

---

### 2. From Volume

Create a box with a specific volume. You provide the volume and two dimensions, and the third dimension is calculated automatically.

**Formula:** `Volume = Length × Width × Height`  
**Calculated:** `Height = Volume / (Length × Width)`

**Steps:**
1. Select "From Volume" mode
2. Enter **Volume** (e.g., 150)
3. Enter **Length (X)** (e.g., 10)
4. Enter **Width (Y)** (e.g., 5)
5. Click "Generate Quader"
6. Height is calculated automatically (e.g., 3)

**Example - Cube with 1m³:**
- Volume: 1
- Length: 1
- Width: 1
- Calculated Height: 1

**Example - 7m³ Cube:**
- Volume: 7
- Length: 1.913
- Width: 1.913
- Calculated Height: 1.913

---

### 3. From Surface Area

Create a box with a specific surface area. You provide the surface area and two dimensions, and the third dimension is calculated automatically.

**Formula:** `Surface Area = 2(LW + LH + WH)`  
**Calculated:** `Height = (Surface Area - 2×L×W) / (2×(L + W))`

**Steps:**
1. Select "From Surface Area" mode
2. Enter **Surface Area** (e.g., 190)
3. Enter **Length (X)** (e.g., 10)
4. Enter **Width (Y)** (e.g., 5)
5. Click "Generate Quader"
6. Height is calculated automatically (e.g., 3)

**Example:**
- Surface Area: 600
- Length: 10
- Width: 10
- Calculated Height: 10

---

## Preset System

Save frequently used configurations for quick access.

### Default Presets

The plugin comes with 7 default presets:
- **Cube 1m³** - Volume-based cube with 1m³
- **Cube 7m³** - Volume-based cube with 7m³
- **Box 1×1×1m** - Direct dimensions (1×1×1m box)
- **Box 2×1.5×0.5m** - Direct dimensions (2×1.5×0.5m box)
- **Surface 100m²** - Surface area-based box (5×5m)
- **Surface 1m²** - Surface area-based box (1×0.2×0.15m)
- **Surface 5m²** - Surface area-based box (2×1×0.15m)

### Loading a Preset

1. Select a preset from the dropdown menu
2. The mode and values are automatically filled
3. Click "Generate Quader" to create the box

### Saving a Preset

1. Configure your desired settings (mode + values)
2. Enter a name in the "Preset name" field
3. Click "Save"
4. The preset is saved and appears in the dropdown

**Storage Location:**  
`%AppData%\QuaderGenerator\QuaderPresets.json` (Windows)  
`~/Library/Application Support/QuaderGenerator/QuaderPresets.json` (Mac)

### Deleting a Preset

1. Select the preset from the dropdown
2. Click "Delete"
3. Confirm deletion in the dialog

**Note:** You can delete custom presets and default presets. Deleted default presets will reappear if the config file is deleted.

---

## Features

### Automatic Calculations
- Volume and surface area are displayed in the Rhino command line after creation
- Missing dimensions are calculated automatically in Volume and Surface Area modes

### Undo Support
All generated boxes support Rhino's undo system (`Ctrl+Z`).

### Input Validation
- All dimensions must be positive numbers
- Invalid inputs show error messages in the status bar
- Surface area must be large enough for the given dimensions

### Placement
All boxes are created at the **world origin (0,0,0)** with axes aligned to the world XYZ coordinate system.

---

## Workflow Examples

### Example 1: Creating a Standard Box
1. Open panel: `QuaderGen`
2. Mode: "Direct Dimensions"
3. Length: 100, Width: 50, Height: 30
4. Click "Generate Quader"
5. Box created at origin

### Example 2: Creating a 500m³ Storage Container
1. Open panel: `QuaderGen`
2. Mode: "From Volume"
3. Volume: 500, Length: 10, Width: 10
4. Click "Generate Quader"
5. Height calculated: 5
6. Save as preset: "Storage Container 500m³"

### Example 3: Creating a Wall Panel with Specific Surface Area
1. Open panel: `QuaderGen`
2. Mode: "From Surface Area"
3. Surface Area: 100, Length: 5, Width: 4
4. Click "Generate Quader"
5. Height calculated automatically
6. Check Rhino command line for exact dimensions

---

## Tips & Tricks

### Quick Access
Add `QuaderGen` to your Rhino toolbar for quick access:
1. Right-click a toolbar
2. "New Button"
3. Command: `QuaderGen`
4. Icon: Use the plugin icon if available

### Keyboard Shortcuts
- `Tab` key moves between input fields
- `Enter` in any input field generates the quader (if all values are valid)
- `Ctrl+Z` to undo the last generated quader

### Working with Presets
- Use descriptive names: "Beam 300x200x50" instead of "Box1"
- Organize by project: "Project A - Wall", "Project A - Column"
- Export the JSON file for backup or sharing with team

### Rhino Command Line Output
After generation, check the command line for:
- Exact dimensions (3 decimal places)
- Calculated volume
- Calculated surface area

---

## Troubleshooting

### Panel Doesn't Open
- Check if plugin is loaded: `_PlugInManager`
- Look for "QuaderGenerator" in the list
- If not loaded, enable it

### Invalid Input Errors
- Ensure all values are positive numbers
- Use decimal point (`.`) not comma (`,`)
- Remove any units (just numbers)

### Surface Area Calculation Errors
If you get "Surface area too small for given dimensions":
- The surface area must be at least `2 × Length × Width`
- Increase surface area or reduce Length/Width

### Presets Not Saving
- Check write permissions to `%AppData%` folder
- Check Rhino command line for error messages
- Manually create folder: `%AppData%\QuaderGenerator`

---

## Advanced Usage

### Config File Location
Presets are stored in JSON format:
```
Windows: C:\Users\[YourName]\AppData\Roaming\QuaderGenerator\QuaderPresets.json
Mac: ~/Library/Application Support/QuaderGenerator/QuaderPresets.json
```

### Manual Preset Editing
You can manually edit the JSON file:
```json
[
  {
    "Name": "My Custom Box",
    "Mode": "dimensions",
    "Length": 10.0,
    "Width": 5.0,
    "Height": 3.0,
    "Volume": 0.0,
    "SurfaceArea": 0.0
  }
]
```

**Modes:**
- `"dimensions"` - Direct dimensions (uses Length, Width, Height)
- `"volume"` - Volume-based (uses Volume, Length, Width)
- `"surface"` - Surface area-based (uses SurfaceArea, Length, Width)

### Sharing Presets
1. Export your `QuaderPresets.json` file
2. Share with colleagues
3. They can replace their file or merge presets manually

---

## API for Developers

See [Agents.md](Agents.md) for development documentation.

### Key Classes:
- `QuaderGenerator.Core.QuaderCalculator` - Mathematical calculations
- `QuaderGenerator.Core.PresetManager` - Preset management
- `QuaderGenerator.UI.QuaderPanel` - Main UI panel

---

## Support & Feedback

- **Issues:** Report bugs on GitHub
- **Feature Requests:** Create an issue with tag "enhancement"
- **Questions:** Ask on Rhino Forum (discourse.mcneel.com)

---

## Version History

### v1.0.0 (October 2025)
- Initial release
- Three generation modes (Direct, Volume, Surface Area)
- Preset system with save/load
- Default presets included
- Undo support
- Cross-platform (Windows/Mac)

---

## License

[Specify license here]

---

**Download:** Get the latest version from [GitHub/food4Rhino]

