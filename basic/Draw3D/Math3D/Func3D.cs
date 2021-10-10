using System;

namespace Draw3D.Math3D
{
    internal static class Func3D
    {
        #region vector functions
        /// <summary>The lenght of Vector4F.</summary>
        public static float Magnitude(Vector4F vector)
        {
            return MathF.Sqrt(MathF.Pow(vector.X, 2) + MathF.Pow(vector.Y, 2) + MathF.Pow(vector.Z, 2));
        }

        /// <summary> Normalized vector (Magnitude = 1).</summary>
        public static Vector4F Normalyze(Vector4F vector)
        {
            var m = Magnitude(vector);
            if (m == 0)
            {
                return vector;
            }

            return new Vector4F(
                vector.X / m,
                vector.Y / m,
                vector.Z / m,
                1
            );
        }

        /// <summary> The cross product of vectors a and b.</summary>
        public static Vector4F Cross(Vector4F a, Vector4F b)
        {
            return new Vector4F(
                a.Y * b.Z - b.Y * a.Z,
                a.Z * b.X - b.Z * a.X,
                a.X * b.Y - b.X * a.Y,
                1
            );
        }
        #endregion
    }
}
