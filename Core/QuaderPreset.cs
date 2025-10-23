using System;

namespace QuaderGenerator.Core
{
    /// <summary>
    /// Represents a saved preset for Quader dimensions
    /// </summary>
    [Serializable]
    public class QuaderPreset
    {
        public string Name { get; set; }
        public string Mode { get; set; } // "dimensions", "volume", "surface"
        public string Unit { get; set; } // "mm", "cm", "m"
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Volume { get; set; }
        public double SurfaceArea { get; set; }

        public QuaderPreset()
        {
            Unit = "m"; // Default to meters
        }

        public QuaderPreset(string name, string mode, string unit, double length, double width, double height = 0, double volume = 0, double surfaceArea = 0)
        {
            Name = name;
            Mode = mode;
            Unit = unit;
            Length = length;
            Width = width;
            Height = height;
            Volume = volume;
            SurfaceArea = surfaceArea;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

