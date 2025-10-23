using System;

namespace QuaderGenerator.Core
{
    /// <summary>
    /// Unit conversion utilities for Quader dimensions
    /// Rhino always works in millimeters internally
    /// </summary>
    public static class UnitConverter
    {
        public enum Unit
        {
            Millimeters,
            Centimeters,
            Meters
        }

        /// <summary>
        /// Convert a linear dimension to millimeters
        /// </summary>
        public static double ToMillimeters(double value, Unit fromUnit)
        {
            switch (fromUnit)
            {
                case Unit.Millimeters:
                    return value;
                case Unit.Centimeters:
                    return value * 10.0;
                case Unit.Meters:
                    return value * 1000.0;
                default:
                    throw new ArgumentException($"Unknown unit: {fromUnit}");
            }
        }

        /// <summary>
        /// Convert a volume to cubic millimeters
        /// </summary>
        public static double VolumeToMillimeters(double value, Unit fromUnit)
        {
            switch (fromUnit)
            {
                case Unit.Millimeters:
                    return value; // mm³
                case Unit.Centimeters:
                    return value * 1000.0; // cm³ to mm³
                case Unit.Meters:
                    return value * 1000000000.0; // m³ to mm³
                default:
                    throw new ArgumentException($"Unknown unit: {fromUnit}");
            }
        }

        /// <summary>
        /// Convert a surface area to square millimeters
        /// </summary>
        public static double SurfaceAreaToMillimeters(double value, Unit fromUnit)
        {
            switch (fromUnit)
            {
                case Unit.Millimeters:
                    return value; // mm²
                case Unit.Centimeters:
                    return value * 100.0; // cm² to mm²
                case Unit.Meters:
                    return value * 1000000.0; // m² to mm²
                default:
                    throw new ArgumentException($"Unknown unit: {fromUnit}");
            }
        }

        /// <summary>
        /// Convert from millimeters to specified unit
        /// </summary>
        public static double FromMillimeters(double valueInMm, Unit toUnit)
        {
            switch (toUnit)
            {
                case Unit.Millimeters:
                    return valueInMm;
                case Unit.Centimeters:
                    return valueInMm / 10.0;
                case Unit.Meters:
                    return valueInMm / 1000.0;
                default:
                    throw new ArgumentException($"Unknown unit: {toUnit}");
            }
        }

        /// <summary>
        /// Get unit symbol
        /// </summary>
        public static string GetUnitSymbol(Unit unit)
        {
            switch (unit)
            {
                case Unit.Millimeters:
                    return "mm";
                case Unit.Centimeters:
                    return "cm";
                case Unit.Meters:
                    return "m";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get volume unit symbol
        /// </summary>
        public static string GetVolumeUnitSymbol(Unit unit)
        {
            switch (unit)
            {
                case Unit.Millimeters:
                    return "mm³";
                case Unit.Centimeters:
                    return "cm³";
                case Unit.Meters:
                    return "m³";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Get surface area unit symbol
        /// </summary>
        public static string GetSurfaceAreaUnitSymbol(Unit unit)
        {
            switch (unit)
            {
                case Unit.Millimeters:
                    return "mm²";
                case Unit.Centimeters:
                    return "cm²";
                case Unit.Meters:
                    return "m²";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Convert surface area from mm² to specified unit
        /// </summary>
        public static double ConvertSurfaceAreaFromMillimeters(double areaMm2, Unit toUnit)
        {
            switch (toUnit)
            {
                case Unit.Millimeters:
                    return areaMm2;
                case Unit.Centimeters:
                    return areaMm2 / 100.0; // mm² to cm² (divide by 10²)
                case Unit.Meters:
                    return areaMm2 / 1000000.0; // mm² to m² (divide by 1000²)
                default:
                    return areaMm2;
            }
        }

        /// <summary>
        /// Convert volume from mm³ to specified unit
        /// </summary>
        public static double ConvertVolumeFromMillimeters(double volumeMm3, Unit toUnit)
        {
            switch (toUnit)
            {
                case Unit.Millimeters:
                    return volumeMm3;
                case Unit.Centimeters:
                    return volumeMm3 / 1000.0; // mm³ to cm³ (divide by 10³)
                case Unit.Meters:
                    return volumeMm3 / 1000000000.0; // mm³ to m³ (divide by 1000³)
                default:
                    return volumeMm3;
            }
        }
    }
}

