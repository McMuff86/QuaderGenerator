### Agents.md - Best Practices for QuaderGenerator Plugin

> **Note:** This document is specifically designed for AI agents and developers working on this project. For general information, see [README.md](README.md). For user documentation, see [Usage.md](Usage.md).

This document outlines best practices for developing, maintaining, and extending the **QuaderGenerator** Rhino plugin. It adheres to Rhino Developer Docs (Rhino 8, .NET 7, as of October 2025). Keep this file up-to-date via Git commits.

## 1. Setup & Development Environment
- **Tools**:
  - Visual Studio 2022 (Community/Professional) with ".NET Desktop Development" workload.
  - Rhino Visual Studio Extension (VSIX): Download from [developer.rhino3d.com/guides/rhinocommon/installing-tools-windows](https://developer.rhino3d.com/guides/rhinocommon/installing-tools-windows).
  - Rhino 8 (preferred; Rhino 7 compatible). Install via [www.rhino3d.com](https://www.rhino3d.com).
  - Yak CLI: Included in Rhino (`C:\Program Files\Rhino 8\System\Yak.exe`).
  - Git for version control (GitHub recommended).
- **Project Setup**:
  - Create: New Project → "RhinoCommon Plug-In for Rhino 3D (C#)".
  - Name: `QuaderGenerator`.
  - Target: .NET 7 (`<TargetFramework>net7.0</TargetFramework>` in .csproj).
  - NuGet Packages: `RhinoCommon` (v8.0.0+), `Eto.Forms` (UI), `xUnit` (tests), `MathNet.Numerics` (optional for math).
- **Version Control**: GitHub repo with `main`, `dev`, `feature/*` branches. Use Semantic Versioning (e.g., v1.0.0).

## 2. Coding Standards & Best Practices
- **Project Structure**:
```
QuaderGenerator/
├── Commands/
│   └── QuaderGenCommand.cs (Command to launch panel)
├── UI/
│   └── QuaderPanel.cs (Eto-based UI with preset management)
├── Core/
│   ├── QuaderCalculator.cs (Math logic for volume/surface calculations)
│   ├── QuaderPreset.cs (Preset data model)
│   └── PresetManager.cs (Save/Load preset logic)
├── QuaderGeneratorPlugin.cs (Main plugin class)
├── Agents.md (This file - developer docs)
├── Usage.md (User documentation)
└── bin/
    └── Debug/
        ├── QuaderGenerator.rhp (Output)
        └── QuaderPresets.json (User presets, stored in %AppData%)
```

- **C# Guidelines**:
- Namespaces: `QuaderGenerator.Commands`, `QuaderGenerator.UI`, `QuaderGenerator.Core`.
- Async: Use `async/await` for UI updates or heavy calculations.
- Logging: Use `Rhino.RhinoApp.WriteLine` for debug output.
- Error Handling: Wrap calculations in `try-catch`, report via `RhinoApp.WriteLineError`.
- Undo: Use `doc.BeginUndoRecord("Add Quader")` for geometry additions.
- **Rhino Integration**:
- Plugin: Extend `Rhino.PlugIns.PlugIn`. Override `OnLoad` for panel registration.
- Commands: Inherit from `Rhino.Commands.Command`. English name: `QuaderGen`.
- UI: Use `Eto.Forms` for cross-platform panels. Register via `Rhino.UI.Panels.RegisterPanel`.
- Geometry: Create quader with `Rhino.Geometry.Box`. Add via `doc.Objects.AddBrep`.
- API: Reference [RhinoCommon API](https://developer.rhino3d.com/api/RhinoCommon/html/R_Rhino.htm). Clarify doubts on [discourse.mcneel.com](https://discourse.mcneel.com).
- **Unit Tests**:
- Use xUnit in `QuaderGenerator.Tests`. Test `QuaderCalculator` (e.g., `CalculateDimensions`).
- Cover edge cases: negative inputs, zero values, unsolvable equations.
- Mocking: Minimal (RhinoCommon is hard to mock). Focus on math logic.
- **Security**:
- Validate inputs: Use `double.TryParse`, ensure positive values.
- No external network calls.
- Cross-platform: Test on Windows/Mac (use Parallels/VM for Mac).

## 3. Testing & Debugging
- **Local Testing**: Run via F5 in VS (attaches to Rhino.exe). Use `RhinoApp.WriteLine` for logs.
- **Unit Tests**: Run xUnit tests in VS Test Explorer. Cover 80%+ of `QuaderCalculator`.
- **Integration Tests**: Manual in Rhino (new file, generate quader, verify surface/volume via `_Area`/`_Volume`).
- **Compatibility**: Test on Rhino 7/8, .NET 7.

## 4. Deployment & Updates
- **Yak Packaging**: Create `manifest.yml` (name: `quadergenerator`, version: `1.0.0`, target: `rh8`). Build with `yak build`.
- **Distribution**: Push to food4Rhino via `yak push`. Host `Agents.md` on GitHub (raw link in README).
- **Updates**: Increment SemVer in `manifest.yml`. Log changes in `Usage.md`.
- **Contributions**: Accept Pull Requests. Track issues on GitHub.

## 5. Maintenance
- **Updates**: Check RhinoCommon updates monthly (via NuGet or [mcneel/rhinocommon-api-docs](https://github.com/mcneel/rhinocommon-api-docs)).
- **Dependencies**: Lock NuGet versions in `packages.lock.json`. Update via VS.
- **Docs**: Commit changes to `Agents.md` with version (e.g., "Update Agents.md to v1.0.1").

**Download**: Get this file via [GitHub Raw Link](<insert-repo-link>/docs/Agents.md) or as part of the Yak package.