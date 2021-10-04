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

            return vector / m;
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

        #region matrix funtions 
        public static Vector4F Mul(Matrix4x4F m, Vector4F v)
        {
            return new Vector4F(
                v.X * m[0].X + v.Y * m[1].X + v.Z * m[2].X + v.W * m[3].X,
                v.X * m[0].Y + v.Y * m[1].Y + v.Z * m[2].Y + v.W * m[3].Y,
                v.X * m[0].Z + v.Y * m[1].Z + v.Z * m[2].Z + v.W * m[3].Z,
                v.X * m[0].W + v.Y * m[1].W + v.Z * m[2].W + v.W * m[3].W
            );            
        }

        public static Matrix4x4F Mul(Matrix4x4F m1, Matrix4x4F m2)
        {
            return new Matrix4x4F(
                Mul(m1, m2[0]),
                Mul(m1, m2[1]),
                Mul(m1, m2[2]),
                Mul(m1, m2[3])
            );
        }
        #endregion
    }
}
