namespace Draw3D.Math3D
{
    internal class Vector4F
    {
        public float X {  get; set; }
        public float Y {  get; set; }
        public float Z {  get; set; }
        public float W {  get; set; }

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


        public static Vector4F Up() => new Vector4F(0, 1, 0);
        public static Vector4F Zero() => new Vector4F(0, 0, 0);

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
    }
}
