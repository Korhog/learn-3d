
namespace Draw3D.Math3D
{
    internal class Vertex
    {
        public Vector4F Position { get; set; }

        public Vector4F Normal {  get; set; }

        public Vector2F UV { get; set; }

        public Vertex(Vector4F position) => Position = position;  

        public Vertex(Vector4F position, Vector4F normal, Vector2F uv)
        {
            Position = position;
            Normal = normal;
            UV = uv; 
        }
    }
}
