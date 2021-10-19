namespace Draw3D.Math3D
{
    internal class Matrix3x3F
    {
        #region matrix data
        public float A11 { get; set; }
        public float A12 { get; set; }
        public float A13 { get; set; }
        public float A21 { get; set; }
        public float A22 { get; set; }
        public float A23 { get; set; }
        public float A31 { get; set; }
        public float A32 { get; set; }
        public float A33 { get; set; }
        #endregion

        Matrix3x3F()
        {
            A11 = A12 = A13 = 0.0f;
            A21 = A22 = A23 = 0.0f;
            A31 = A32 = A33 = 0.0f;
        }

        public Matrix3x3F(float a11, float a12, float a13, float a21, float a22, float a23, float a31, float a32, float a33)
        {
            A11 = a11;
            A12 = a12;
            A13 = a13;
            A21 = a21;
            A22 = a22;
            A23 = a23;
            A31 = a31;
            A32 = a32;
            A33 = a33;
        }

        public static float Determinant(Matrix3x3F m)
        {
            var a = m.A11 * m.A22 * m.A33 + m.A12 * m.A23 * m.A31 + m.A13 * m.A21 * m.A32;
            var b = m.A13 * m.A22 * m.A31 + m.A11 * m.A23 * m.A32 + m.A12 * m.A21 * m.A33;

            return a - b;
        }
    }
}
