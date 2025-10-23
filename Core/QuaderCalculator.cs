using System;

namespace QuaderGenerator.Core
{
    /// <summary>
    /// Mathematical calculations for Quader (box) geometry
    /// </summary>
    public static class QuaderCalculator
    {
        /// <summary>
        /// Calculate volume from dimensions
        /// Volume = length × width × height
        /// </summary>
        public static double CalculateVolume(double length, double width, double height)
        {
            if (length <= 0 || width <= 0 || height <= 0)
                throw new ArgumentException("All dimensions must be positive");
            
            return length * width * height;
        }

        /// <summary>
        /// Calculate surface area from dimensions
        /// Surface Area = 2(lw + lh + wh)
        /// </summary>
        public static double CalculateSurfaceArea(double length, double width, double height)
        {
            if (length <= 0 || width <= 0 || height <= 0)
                throw new ArgumentException("All dimensions must be positive");
            
            return 2 * (length * width + length * height + width * height);
        }

        /// <summary>
        /// Calculate missing dimension from volume and two known dimensions
        /// </summary>
        public static double CalculateDimensionFromVolume(double volume, double dim1, double dim2)
        {
            if (volume <= 0 || dim1 <= 0 || dim2 <= 0)
                throw new ArgumentException("Volume and dimensions must be positive");
            
            // volume = dim1 × dim2 × dim3
            // dim3 = volume / (dim1 × dim2)
            return volume / (dim1 * dim2);
        }

        /// <summary>
        /// Calculate missing dimension from surface area and two known dimensions
        /// Surface Area = 2(lw + lh + wh)
        /// Solve for the third dimension
        /// </summary>
        public static double CalculateDimensionFromSurfaceArea(double surfaceArea, double dim1, double dim2)
        {
            if (surfaceArea <= 0 || dim1 <= 0 || dim2 <= 0)
                throw new ArgumentException("Surface area and dimensions must be positive");
            
            // A = 2(d1*d2 + d1*d3 + d2*d3)
            // A = 2*d1*d2 + 2*d1*d3 + 2*d2*d3
            // A = 2*d1*d2 + 2*d3*(d1 + d2)
            // A - 2*d1*d2 = 2*d3*(d1 + d2)
            // d3 = (A - 2*d1*d2) / (2*(d1 + d2))
            
            double numerator = surfaceArea - 2 * dim1 * dim2;
            double denominator = 2 * (dim1 + dim2);
            
            if (denominator == 0)
                throw new ArgumentException("Invalid dimensions for surface area calculation");
            
            double result = numerator / denominator;
            
            if (result <= 0)
                throw new ArgumentException("Surface area too small for given dimensions");
            
            return result;
        }

        /// <summary>
        /// Validate that dimensions are positive
        /// </summary>
        public static bool AreDimensionsValid(double length, double width, double height)
        {
            return length > 0 && width > 0 && height > 0;
        }

        /// <summary>
        /// Calculate dimensions for a cube with given volume
        /// </summary>
        public static double CalculateCubeSideFromVolume(double volume)
        {
            if (volume <= 0)
                throw new ArgumentException("Volume must be positive");
            
            return Math.Pow(volume, 1.0 / 3.0);
        }

        /// <summary>
        /// Calculate dimensions for a cube with given surface area
        /// </summary>
        public static double CalculateCubeSideFromSurfaceArea(double surfaceArea)
        {
            if (surfaceArea <= 0)
                throw new ArgumentException("Surface area must be positive");
            
            // For a cube: A = 6*s²
            // s = sqrt(A/6)
            return Math.Sqrt(surfaceArea / 6.0);
        }
    }
}

