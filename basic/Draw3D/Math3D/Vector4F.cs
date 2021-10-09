namespace Draw3D.Math3D
{
    internal class Vector4F
    {
        public float X {  get; private set; }
        public float Y {  get; private set; }
        public float Z {  get; private set; }
        public float W {  get; private set; }

        public Vector4F()
        {
            X = Y = Z = W = 0.0f;
        }

        public Vector4F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1;
        }

        public Vector4F(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }


        public static Vector4F Up => new Vector4F(0, 1, 0);
        public static Vector4F Zero => new Vector4F(0, 0, 0);

        // Vector operators
        public static Vector4F operator +(Vector4F a, Vector4F b)
        {
            return new Vector4F(
                a.X + b.X,
                a.Y + b.Y,
                a.Z + b.Z,
                a.W + b.W
            );
        }

        public static Vector4F operator -(Vector4F a, Vector4F b)
        {
            return new Vector4F(
                a.X - b.X,
                a.Y - b.Y,
                a.Z - b.Z,
                a.W - b.W
            );
        }

        public static Vector4F operator *(Vector4F a, float s)
        {
            return new Vector4F(
                a.X * s,
                a.Y * s,
                a.Z * s,
                a.W * s
            );
        }
        public static Vector4F operator /(Vector4F a, float s)
        {
            return new Vector4F(
                a.X / s,
                a.Y / s,
                a.Z / s,
                a.W / s
            );
        }

        public static Vector4F Transform(Matrix4x4F m, Vector4F v)
        {
            var x = v.X * m.A11 + v.Y * m.A21 + v.Z * m.A31 + v.W * m.A41;
            var y = v.X * m.A12 + v.Y * m.A22 + v.Z * m.A32 + v.W * m.A42;
            var z = v.X * m.A13 + v.Y * m.A23 + v.Z * m.A33 + v.W * m.A43;
            var w = v.X * m.A14 + v.Y * m.A24 + v.Z * m.A34 + v.W * m.A44;

            if(w != 0)
            {
                x = x / w;
                y = y / w;
                z = z / w;
                w = w / w;
            }

            return new Vector4F(x, y, z, w);
        }
    }
}
