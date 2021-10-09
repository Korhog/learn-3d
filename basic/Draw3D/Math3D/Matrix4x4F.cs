using System;
using Windows.Foundation;

namespace Draw3D.Math3D
{
    internal class Matrix4x4F
    {
        #region matrix data
        public float A11 { get; private set; }
        public float A12 { get; private set; }
        public float A13 { get; private set; }
        public float A14 { get; private set; }
        public float A21 { get; private set; }
        public float A22 { get; private set; }
        public float A23 { get; private set; }
        public float A24 { get; private set; }
        public float A31 { get; private set; }
        public float A32 { get; private set; }
        public float A33 { get; private set; }
        public float A34 { get; private set; }
        public float A41 { get; private set; }
        public float A42 { get; private set; }
        public float A43 { get; private set; }
        public float A44 { get; private set; }
        #endregion

        Matrix4x4F()
        {
            A11 = A12 = A13 = A14 = 0.0f;
            A21 = A22 = A23 = A24 = 0.0f;
            A31 = A32 = A33 = A34 = 0.0f;
            A41 = A42 = A43 = A44 = 0.0f;
        }

        public static Matrix4x4F Identity()
        {
            var matrix = new Matrix4x4F();
            matrix.A11 = 1.0f;
            matrix.A22 = 1.0f;
            matrix.A33 = 1.0f;
            matrix.A44 = 1.0f;
            return matrix;
        }


        public static Matrix4x4F Perspective(float fov, float aspect, float near, float far)
        {
            var angle = fov * MathF.PI / 180.0f;
            var n = 1.0f / MathF.Tan(angle);

            var result = Identity();

            result.A11 = n;
            result.A22 = n;
            result.A34 = 1.0f;
            result.A44 = 0.0f;

            return result;          
        }

        public static Matrix4x4F LookAt(Vector4F eye, Vector4F target, Vector4F up)
        {
            var zaxis = Func3D.Normalyze(target - eye);
            var xaxis = Func3D.Normalyze(Func3D.Cross(up, zaxis));
            var yaxis = Func3D.Normalyze(Func3D.Cross(zaxis, xaxis));

            var orientation = Identity();
            
            orientation.A11 = xaxis.X;
            orientation.A21 = xaxis.Y;
            orientation.A31 = xaxis.Z;

            orientation.A12 = yaxis.X;
            orientation.A22 = yaxis.Y;
            orientation.A32 = yaxis.Z;

            orientation.A13 = zaxis.X;
            orientation.A23 = zaxis.Y;
            orientation.A33 = zaxis.Z;

            var translation = Translation(-eye.X, -eye.Y, -eye.Z);

            return Multiply(translation, orientation);
            //return orientation;
        }

        public static Matrix4x4F Translation(float x, float y, float z)
        {
            var translation = Identity();

            translation.A41 = x;
            translation.A42 = y;
            translation.A43 = z;

            return translation;          
        }

        public static Matrix4x4F RotationX(float angle)
        {
            var m = Identity();

            m.A22 = MathF.Cos(angle); 
            m.A23 = -MathF.Sin(angle);
            m.A32 = MathF.Sin(angle);
            m.A33 = MathF.Cos(angle);

            return m;
        }

        public static Matrix4x4F RotationY(float angle)
        {
            var m = Identity();

            m.A11 = MathF.Cos(angle);
            m.A13 = MathF.Sin(angle);
            m.A31 = -MathF.Sin(angle);
            m.A33 = MathF.Cos(angle);

            return m;
        }

        public static Matrix4x4F RotationZ(float angle)
        {
            var m = Identity();

            m.A11 = MathF.Cos(angle);
            m.A12 = -MathF.Sin(angle);
            m.A21 = MathF.Sin(angle);
            m.A22 = MathF.Cos(angle);

            return m;
        }

        public static Matrix4x4F Rotation(float x, float y, float z)
        {
            var Rz = RotationZ(z);
            var Ry = RotationY(y);
            var Rx = RotationX(x);

            return Multiply(Rx, Multiply(Rz, Ry));
        }

        public static Matrix4x4F WorldToScreen(Size size)
        {
            var w = (float)size.Width / 2.0f;
            var h = (float)size.Height / 2.0f;

            var a = w > h ? h : w;

            //var x = w + p.X * a;
            //var y = h - p.Y * a;
            //return new Vector2F(x, y);

            // a  0  0  0
            // 0 -a  0  0 
            // 0  0  1  0 
            // w  h  0  1

            var m = Identity();

            m.A11 = a;
            m.A22 = -a;
            m.A41 = w;
            m.A42 = h;

            return m;
        }

        public static Matrix4x4F Multiply(Matrix4x4F a, Matrix4x4F b)
        {
            var result = new Matrix4x4F();

            result.A11 = a.A11 * b.A11 + a.A12 * b.A21 + a.A13 * b.A31 + a.A14 * b.A41;
            result.A12 = a.A11 * b.A12 + a.A12 * b.A22 + a.A13 * b.A32 + a.A14 * b.A42;
            result.A13 = a.A11 * b.A13 + a.A12 * b.A23 + a.A13 * b.A33 + a.A14 * b.A43;
            result.A14 = a.A11 * b.A14 + a.A12 * b.A24 + a.A13 * b.A34 + a.A14 * b.A44;

            result.A21 = a.A21 * b.A11 + a.A22 * b.A21 + a.A23 * b.A31 + a.A24 * b.A41;
            result.A22 = a.A21 * b.A12 + a.A22 * b.A22 + a.A23 * b.A32 + a.A24 * b.A42;
            result.A23 = a.A21 * b.A13 + a.A22 * b.A23 + a.A23 * b.A33 + a.A24 * b.A43;
            result.A24 = a.A21 * b.A14 + a.A22 * b.A24 + a.A23 * b.A34 + a.A24 * b.A44;

            result.A31 = a.A31 * b.A11 + a.A32 * b.A21 + a.A33 * b.A31 + a.A34 * b.A41;
            result.A32 = a.A31 * b.A12 + a.A32 * b.A22 + a.A33 * b.A32 + a.A34 * b.A42;
            result.A33 = a.A31 * b.A13 + a.A32 * b.A23 + a.A33 * b.A33 + a.A34 * b.A43;
            result.A34 = a.A31 * b.A14 + a.A32 * b.A24 + a.A33 * b.A34 + a.A34 * b.A44;

            result.A41 = a.A41 * b.A11 + a.A42 * b.A21 + a.A43 * b.A31 + a.A44 * b.A41;
            result.A42 = a.A41 * b.A12 + a.A42 * b.A22 + a.A43 * b.A32 + a.A44 * b.A42;
            result.A43 = a.A41 * b.A13 + a.A42 * b.A23 + a.A43 * b.A33 + a.A44 * b.A43;
            result.A44 = a.A41 * b.A14 + a.A42 * b.A24 + a.A43 * b.A34 + a.A44 * b.A44;

            return result;

        }
    }
}
