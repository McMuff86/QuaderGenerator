# Changelog

All notable changes to QuaderGenerator will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- ✅ Icon integration (plugin-utility.ico)
- ✅ Extended default presets (added 1m² and 5m² surface area boxes)
- ✅ Fixed Object Info Box scrollbar issues (auto-expanding height)
- ✅ Updated documentation (README.md, Usage.md)

### Planned
- Unit tests with xUnit
- Batch generation mode
- Custom placement (not just origin)
- Export presets as JSON
- Material assignment

---

## [1.0.0] - 2025-10-23

### Added
- ✅ Three generation modes:
  - Direct Dimensions
  - From Volume (calculate height from volume)
  - From Surface Area (calculate height from surface)
- ✅ Smart unit system (mm, cm, m) with automatic conversion
- ✅ Preset management system
  - 5 default presets
  - Save/Load custom presets
  - Unit-aware presets
  - JSON-based storage
- ✅ Object analysis tool
  - Real-time info for selected objects
  - Support for: Brep, Extrusion, Curve, Surface, Mesh
  - Shows: Volume, Surface Area, Length, etc.
- ✅ Comprehensive UI with GroupBoxes
- ✅ Cross-platform support (Windows/Mac)
- ✅ Undo/Redo support
- ✅ Detailed debug output in Rhino command line

### Technical
- Built with RhinoCommon 8.0.23304.9001
- Eto.Forms 2.8.3 for UI
- System.Text.Json 8.0.5 for config
- Multi-target: .NET 7.0 and .NET Framework 4.8
- Yak package creation integrated

### Documentation
- README.md for GitHub
- Usage.md for end-users
- Agents.md for AI agents and developers
- Inline code documentation

---

## [0.1.0] - 2025-10-22

### Added
- Initial project structure
- Basic command registration
- Simple box generation at origin

---

## Release Notes Template

### [X.Y.Z] - YYYY-MM-DD

#### Added
- New features

#### Changed
- Changes in existing functionality

#### Deprecated
- Soon-to-be removed features

#### Removed
- Removed features

#### Fixed
- Bug fixes

#### Security
- Security fixes

---

[Unreleased]: https://github.com/yourusername/QuaderGenerator/compare/v1.0.0...HEAD
[1.0.0]: https://github.com/yourusername/QuaderGenerator/releases/tag/v1.0.0
[0.1.0]: https://github.com/yourusername/QuaderGenerator/releases/tag/v0.1.0

