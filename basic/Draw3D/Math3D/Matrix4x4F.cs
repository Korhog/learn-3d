using System;
using System.Collections.Generic;
using System.Linq;
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

        public Matrix4x4F(System.Numerics.Matrix4x4 m)
        {
            A11 = m.M11; 
            A12 = m.M12;
            A13 = m.M13;
            A14 = m.M14;
            A21 = m.M21;
            A22 = m.M22;
            A23 = m.M23;
            A24 = m.M24;
            A31 = m.M31;
            A32 = m.M32;
            A33 = m.M33;
            A34 = m.M34;
            A41 = m.M41;
            A42 = m.M42;
            A43 = m.M43;
            A44 = m.M44;  
        }

        public static Matrix4x4F operator /(Matrix4x4F m, float s)
        {
            var result = new Matrix4x4F();

            result.A11 = m.A11 / s;
            result.A12 = m.A12 / s;
            result.A13 = m.A13 / s;
            result.A14 = m.A14 / s;

            result.A21 = m.A21 / s;
            result.A22 = m.A22 / s;
            result.A23 = m.A23 / s;
            result.A24 = m.A24 / s;

            result.A31 = m.A31 / s;
            result.A32 = m.A32 / s;
            result.A33 = m.A33 / s;
            result.A34 = m.A34 / s;

            result.A41 = m.A41 / s;
            result.A42 = m.A42 / s;
            result.A43 = m.A43 / s;
            result.A44 = m.A44 / s;

            return result;

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

        public static Matrix4x4F MinorTest()
        {
            var m = new Matrix4x4F();

            m.A11 = 11;
            m.A12 = 12;
            m.A13 = 13;
            m.A14 = 14;

            m.A21 = 21;
            m.A22 = 22;
            m.A23 = 23;
            m.A24 = 24;

            m.A31 = 31;
            m.A32 = 32;
            m.A33 = 33;
            m.A34 = 34;

            m.A41 = 41;
            m.A42 = 42;
            m.A43 = 43;
            m.A44 = 44;

            return m;
        }

        public static Matrix4x4F DeterminantTest()
        {
            var m = new Matrix4x4F();

            m.A11 =  2;
            m.A12 =  4;
            m.A13 =  0;
            m.A14 = -2;

            m.A21 =  3;
            m.A22 = -1;
            m.A23 =  4;
            m.A24 =  1;

            m.A31 =  1;
            m.A32 =  2;
            m.A33 =  0;
            m.A34 =  0;

            m.A41 =  0;
            m.A42 =  5;
            m.A43 =  1;
            m.A44 =  2;

            return m;
        }

        public static Matrix4x4F Perspective(float fov, float near, float far)
        {
            var angle = fov * MathF.PI / 180.0f;
            var n = 1.0f / MathF.Tan(angle);

            var a = (far + near) / (far - near);
            var b = -2 * far * near / (far - near); 

            var result = Identity();

            result.A11 = n;
            result.A22 = n;
            result.A34 = 1.0f;
            result.A44 = 0.0f;

            // fix Z

            result.A33 = a;
            result.A43 = b;

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

        public static Matrix4x4F Transpose(Matrix4x4F m)
        {
            var result = new Matrix4x4F();

            result.A11 = m.A11;
            result.A21 = m.A12;
            result.A31 = m.A13;
            result.A41 = m.A14;

            result.A12 = m.A21;
            result.A22 = m.A22;
            result.A32 = m.A23;
            result.A42 = m.A24;

            result.A13 = m.A31;
            result.A23 = m.A32;
            result.A33 = m.A33;
            result.A43 = m.A34;

            result.A14 = m.A41;   
            result.A24 = m.A42;
            result.A34 = m.A43;
            result.A44 = m.A44;

            return result;
        }

        public static float Determinant(Matrix4x4F m)
        {
            var d = 0f;

            var minor = Minor(m, 0, 0);
            var det = Matrix3x3F.Determinant(minor);
            var a = m.A11 * det;
            d += a;

            minor = Minor(m, 0, 1);
            det = Matrix3x3F.Determinant(minor);
            a = -m.A12 * det;
            d += a;

            minor = Minor(m, 0, 2);
            det = Matrix3x3F.Determinant(minor);
            a = m.A13 * det;
            d += a;

            minor = Minor(m, 0, 3);
            det = Matrix3x3F.Determinant(minor);
            a = -m.A14 * det;
            d += a;

            return d;
        }

        public static Matrix3x3F Minor(Matrix4x4F m, int i, int j)
        {
            var rows = new int[] { 0, 1, 2, 3 }.Where(x => x != i).ToArray();
            var cols = new int[] { 0, 1, 2, 3 }.Where(x => x != j).ToArray();

            var mx = new float[4, 4]
            {
               { m.A11, m.A12, m.A13, m.A14 },
               { m.A21, m.A22, m.A23, m.A24 },
               { m.A31, m.A32, m.A33, m.A34 },
               { m.A41, m.A42, m.A43, m.A44 },
            };

            var a11 = mx[rows[0], cols[0]];
            var a12 = mx[rows[0], cols[1]];
            var a13 = mx[rows[0], cols[2]];

            var a21 = mx[rows[1], cols[0]];
            var a22 = mx[rows[1], cols[1]];
            var a23 = mx[rows[1], cols[2]];

            var a31 = mx[rows[2], cols[0]];
            var a32 = mx[rows[2], cols[1]];
            var a33 = mx[rows[2], cols[2]];

            return new Matrix3x3F(a11, a12, a13, a21, a22, a23, a31, a32, a33);
        }

        public static bool Invert(Matrix4x4F m, out Matrix4x4F inverted)
        {
            // result = A* / det
            // A* = T(C)

            inverted = null;
            var det = Determinant(m);
            if(det == 0)
            {
                return false;
            }

            var c = new Matrix4x4F();

            c.A11 =  Matrix3x3F.Determinant(Minor(m, 0, 0));
            c.A12 = -Matrix3x3F.Determinant(Minor(m, 0, 1));
            c.A13 =  Matrix3x3F.Determinant(Minor(m, 0, 2));
            c.A14 = -Matrix3x3F.Determinant(Minor(m, 0, 3));

            c.A21 = -Matrix3x3F.Determinant(Minor(m, 1, 0));
            c.A22 =  Matrix3x3F.Determinant(Minor(m, 1, 1));
            c.A23 = -Matrix3x3F.Determinant(Minor(m, 1, 2));
            c.A24 =  Matrix3x3F.Determinant(Minor(m, 1, 3));

            c.A31 =  Matrix3x3F.Determinant(Minor(m, 2, 0));
            c.A32 = -Matrix3x3F.Determinant(Minor(m, 2, 1));
            c.A33 =  Matrix3x3F.Determinant(Minor(m, 2, 2));
            c.A34 = -Matrix3x3F.Determinant(Minor(m, 2, 3));

            c.A41 = -Matrix3x3F.Determinant(Minor(m, 3, 0));
            c.A42 =  Matrix3x3F.Determinant(Minor(m, 3, 1));
            c.A43 = -Matrix3x3F.Determinant(Minor(m, 3, 2));
            c.A44 =  Matrix3x3F.Determinant(Minor(m, 3, 3));

            inverted = Transpose(c) / det;

            return true;
        } 
    }
}
