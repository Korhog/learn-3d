using System;

namespace Draw3D.Math3D
{
    internal class Matrix4x4F
    {
        private Vector4F[] _data;

        public Vector4F this[int index] => _data[index];

        public Matrix4x4F()
        {
            _data = new Vector4F[4]
            {
                new Vector4F(),
                new Vector4F(),
                new Vector4F(),
                new Vector4F()
            };
        }

        public Matrix4x4F(Vector4F a, Vector4F b, Vector4F c, Vector4F d)
        {
            _data = new Vector4F[4] { a, b, c, d }; 
        }

        public static Matrix4x4F Perspective(float fov, float aspect, float near, float far)
        {
            var a = 1f / MathF.Tan(fov / 2f);
            var b = a / aspect;
            var c = (far + near) / (far - near);
            var d = (-2 * far * near) / (far - near);

            return new Matrix4x4F(
                new Vector4F(b, 0, 0, 0),
                new Vector4F(0, a, 0, 0),
                new Vector4F(0, 0, c, 0),
                new Vector4F(0, 0, d, 0)
            );
        }

        public static Matrix4x4F LookAt(Vector4F eye, Vector4F target, Vector4F up)
        {
            var zaxis = Func3D.Normalyze(target - eye);
            var xaxis = Func3D.Normalyze(Func3D.Cross(up, zaxis));
            var yaxis = Func3D.Normalyze(Func3D.Cross(zaxis, xaxis));

            var orientation = new Matrix4x4F(
                new Vector4F(xaxis.X, yaxis.X, zaxis.X, 0),
                new Vector4F(xaxis.Y, yaxis.Y, zaxis.Y, 0),
                new Vector4F(xaxis.Z, yaxis.Z, zaxis.Z, 0),
                new Vector4F(0,0,0,1)
            );

            var translation = Matrix4x4F.Translation(-eye.X, -eye.Y, -eye.Z);
            return Func3D.Mul(orientation, translation);
        }

        public static Matrix4x4F Translation(float x, float y, float z)
        {
            return new Matrix4x4F(
                new Vector4F(1, 0, 0, 0),
                new Vector4F(0, 1, 0, 0),
                new Vector4F(0, 0, 1, 0),
                new Vector4F(x, y, z, 1)
            );
        }
    }
}
