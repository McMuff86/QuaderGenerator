using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Rhino;

namespace QuaderGenerator.Core
{
    /// <summary>
    /// Manages saving and loading of Quader presets
    /// </summary>
    public class PresetManager
    {
        private const string ConfigFileName = "QuaderPresets.json";
        private static string ConfigFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "QuaderGenerator",
            ConfigFileName
        );

        /// <summary>
        /// Load all presets from config file
        /// </summary>
        public static List<QuaderPreset> LoadPresets()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                {
                    // Return default presets
                    return GetDefaultPresets();
                }

                var json = File.ReadAllText(ConfigFilePath);
                var presets = JsonSerializer.Deserialize<List<QuaderPreset>>(json);
                
                if (presets == null || presets.Count == 0)
                {
                    return GetDefaultPresets();
                }

                return presets;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"Error loading presets: {ex.Message}");
                return GetDefaultPresets();
            }
        }

        /// <summary>
        /// Save presets to config file
        /// </summary>
        public static bool SavePresets(List<QuaderPreset> presets)
        {
            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Serialize to JSON
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                var json = JsonSerializer.Serialize(presets, options);
                
                // Write to file
                File.WriteAllText(ConfigFilePath, json);
                
                RhinoApp.WriteLine($"Presets saved to: {ConfigFilePath}");
                return true;
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"ERROR: Failed to save presets: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Add a new preset
        /// </summary>
        public static bool AddPreset(QuaderPreset preset)
        {
            try
            {
                var presets = LoadPresets();
                
                // Check if preset with same name exists
                var existing = presets.FirstOrDefault(p => p.Name.Equals(preset.Name, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    presets.Remove(existing);
                }
                
                presets.Add(preset);
                return SavePresets(presets);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"ERROR: Failed to add preset: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Delete a preset by name
        /// </summary>
        public static bool DeletePreset(string name)
        {
            try
            {
                var presets = LoadPresets();
                var preset = presets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                
                if (preset == null)
                {
                    return false;
                }
                
                presets.Remove(preset);
                return SavePresets(presets);
            }
            catch (Exception ex)
            {
                RhinoApp.WriteLine($"ERROR: Failed to delete preset: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get default presets (values stored in their display units)
        /// </summary>
        private static List<QuaderPreset> GetDefaultPresets()
        {
            return new List<QuaderPreset>
            {
                // 1m³ cube in meters
                new QuaderPreset("Cube 1m³", "volume", "m", 1, 1, 0, 1, 0),
                
                // 7m³ cube: side = ³√7 ≈ 1.9129m
                new QuaderPreset("Cube 7m³", "volume", "m", 1.9129, 1.9129, 0, 7, 0),
                
                // Box 1m × 1m × 1m in meters
                new QuaderPreset("Box 1×1×1m", "dimensions", "m", 1, 1, 1, 0, 0),
                
                // Box 2m × 1.5m × 0.5m in meters
                new QuaderPreset("Box 2×1.5×0.5m", "dimensions", "m", 2, 1.5, 0.5, 0, 0),
                
                // 100m² surface area: 5m × 5m box
                new QuaderPreset("Surface 100m²", "surface", "m", 5, 5, 0, 0, 100)
            };
        }
    }
}

