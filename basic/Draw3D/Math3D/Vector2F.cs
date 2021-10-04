namespace Draw3D.Math3D
{
    internal class Vector2F
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2F()
        {
            X = Y = 0.0f;
        }

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2F(Vector4F v)
        {
            X = v.X;
            Y = v.Y;
        }
    }
}
