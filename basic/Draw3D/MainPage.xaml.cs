using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Draw3D.Math3D;
using Windows.UI;
using System;
using Microsoft.Graphics.Canvas;

namespace Draw3D
{
    public sealed partial class MainPage : Page
    {
        private float angle = 0.0f;

        Vector4F[] box = new Vector4F[8]
        {
            new Vector4F(-1,  1, -1, 1), // 0 A C E
            new Vector4F(-1,  1,  1, 1), // 1 B C E
            new Vector4F(-1, -1,  1, 1), // 2 B C F 
            new Vector4F(-1, -1, -1, 1), // 3 A C F
            new Vector4F( 1,  1, -1, 1), // 4 A D E
            new Vector4F( 1,  1,  1, 1), // 5 B D E
            new Vector4F( 1, -1,  1, 1), // 6 B D F
            new Vector4F( 1, -1, -1, 1)  // 7 A D F
        };

        int[] ind = new int[] { 0, 3, 7, 4, 1, 2, 6, 5, 0, 1, 2, 3, 4, 5, 6, 7, 0, 1, 5, 4, 2, 3, 7, 6 };

        Vertex[] vertices = new Vertex[8]
        {
            new Vertex(new Vector4F(-1,  1, -1, 1)), // 0 A C E
            new Vertex(new Vector4F(-1,  1,  1, 1)), // 1 B C E
            new Vertex(new Vector4F(-1, -1,  1, 1)), // 2 B C F 
            new Vertex(new Vector4F(-1, -1, -1, 1)), // 3 A C F
            new Vertex(new Vector4F( 1,  1, -1, 1)), // 4 A D E
            new Vertex(new Vector4F( 1,  1,  1, 1)), // 5 B D E
            new Vertex(new Vector4F( 1, -1,  1, 1)), // 6 B D F
            new Vertex(new Vector4F( 1, -1, -1, 1) ) // 7 A D F
        };

        int[] indicies = new int[]
        {
            3, 0, 7, 7, 0, 4,
            6, 5, 2, 2, 5, 1,
            2, 1, 3, 3, 1, 0,
            7, 4, 6, 6, 4, 5,
            0, 1, 5, 0, 5, 4,
            7, 6, 3, 3, 6, 2
        };

        Vector4F eye = new Vector4F(0, 2, -5, 0);
        Matrix4x4F viewMatrix;
        Matrix4x4F perspectiveMatrix;
        Matrix4x4F worldToScreenMatrix;
        Matrix4x4F mvp;
        Matrix4x4F rotation;

        public MainPage()
        {
            this.InitializeComponent();
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);

            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
        } 

        private void OnAnimatedDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var a = angle * MathF.PI / 180.0f;
            rotation = Matrix4x4F.Rotation(a, a, a);
            //rotation = Matrix4x4F.RotationY(a);

            /*

            Vector4F p2;

            for (var side = 0; side < 6; side++)
            {
                var s = side * 4;
                for (var i = 0; i < 4; i++)
                {
                    var p1 = box[ind[s + i]];
                    p2 = i == 3 ? box[ind[s]] : box[ind[s + i + 1]];


                    var pt1 = Vector4F.Transform(rotation, p1);
                    pt1 = Vector4F.Transform(mvp, pt1);
                    //pt1 = Vector4F.Transform(viewMatrix, pt1);
                    //pt1 = Vector4F.Transform(perspectiveMatrix, pt1);
                    //pt1 = Vector4F.Transform(worldToScreenMatrix, pt1);

                    var pt2 = Vector4F.Transform(rotation, p2);
                    pt2 = Vector4F.Transform(mvp, pt2);
                    //pt2 = Vector4F.Transform(viewMatrix, pt2);
                    //pt2 = Vector4F.Transform(perspectiveMatrix, pt2);
                    //pt2 = Vector4F.Transform(worldToScreenMatrix, pt2);

                    args.DrawingSession.DrawLine(pt1.X, pt1.Y, pt2.X, pt2.Y, Colors.CornflowerBlue, 2.0f);
                }
            }
            */

            DrawIndicies(vertices, indicies, args.DrawingSession);

            angle += 1f;
            if (angle > 360.0f)
            {
                angle -= 360.0f;
            }            
        }

        private void DrawIndicies(Vertex[] vertexes, int[] indicies, CanvasDrawingSession session)
        {
            var trianglesCount = indicies.Length / 3;
            if (trianglesCount < 1)        
                return;

            for(int i = 0; i < trianglesCount; i++)
            {
                var a = vertexes[indicies[i * 3]];
                var b = vertexes[indicies[i * 3 + 1]];
                var c = vertexes[indicies[i * 3 + 2]];

                DrawTriangle(a, b, c, session);    
            }           

        }

        private void DrawTriangle(Vertex a, Vertex b, Vertex c, CanvasDrawingSession session)
        {
            // get normal
            var faceNormal = Func3D.Normalyze(
                Func3D.Cross(
                    b.Position - a.Position,
                    c.Position - a.Position
                ));

            // backface culling
            var m = Matrix4x4F.Multiply(Matrix4x4F.Multiply(rotation, viewMatrix), perspectiveMatrix);

            var pa = Vector4F.Transform(m, a.Position);
            var pb = Vector4F.Transform(m, b.Position);
            var pc = Vector4F.Transform(m, c.Position);

            var pbn = Func3D.Normalyze(pb - pa);
            var pcn = Func3D.Normalyze(pc - pa);

            var screenNormal = Func3D.Cross(pbn, pcn);

            if (screenNormal.Z < 0)
            {
                pa = Vector4F.Transform(worldToScreenMatrix, pa);
                pb = Vector4F.Transform(worldToScreenMatrix, pb);
                pc = Vector4F.Transform(worldToScreenMatrix, pc);

                // rasterize 
                var (minY, maxY) = Area(pa, pb, pc);

                var light = Vector4F.Angle(
                    Vector4F.Transform(rotation, faceNormal),
                    new Vector4F(0, 0, -1));

                var color = Light(Colors.YellowGreen, light);

                for(int i = (int)minY; i < (int)maxY; i++)
                {
                    float?[] points = new float?[3]
                    {
                        XOL(pa, pb, i),
                        XOL(pb, pc, i),
                        XOL(pc, pa, i)
                    };

                    float? min = null;
                    float? max = null;

                    foreach (var p in points)
                    {
                        if (p.HasValue)
                        {
                            // session.FillCircle(p.Value, i, 3, Colors.Red);

                            min = min.HasValue ? (min.Value > p.Value ? p : min) : p;
                            max = max.HasValue ? (max.Value < p.Value ? p : max) : p;
                        }
                    }

                    if (min.HasValue && max.HasValue)
                    {
                        session.DrawLine(
                            min.Value,
                            i,
                            max.Value,
                            i,
                            color,
                            1.0f);
                    }
                }

                //draw edges
                DrawLine(a, b, session, color);
                DrawLine(b, c, session, color);
                DrawLine(c, a, session, color);

                //// draw normals
                //DrawLine(a.Position, a.Position + faceNormal, session, Colors.YellowGreen);
                //DrawLine(b.Position, b.Position + faceNormal, session, Colors.YellowGreen);
                //DrawLine(c.Position, c.Position + faceNormal, session, Colors.YellowGreen);
            }
        }

        private void DrawLine(Vertex a, Vertex b, CanvasDrawingSession session, Color? color = null)
        {
            DrawLine(a.Position, b.Position, session, color ?? Colors.CornflowerBlue);
        }

        private void DrawLine(Vector4F a, Vector4F b, CanvasDrawingSession session, Color color)
        {
            var pt1 = Vector4F.Transform(rotation, a);
            pt1 = Vector4F.Transform(mvp, pt1);

            var pt2 = Vector4F.Transform(rotation, b);
            pt2 = Vector4F.Transform(mvp, pt2);

            session.DrawLine(
                pt1.X,
                pt1.Y,
                pt2.X,
                pt2.Y,
                color, 1.0f);
        }

        private float? XOL(Vector4F a, Vector4F b, float y)
        {
            if (y > MathF.Max(a.Y, b.Y) || y < MathF.Min(a.Y, b.Y))
                return null;

            var dy = MathF.Abs(a.Y - y);
            var len = MathF.Abs(a.Y - b.Y);

            if (len == 0)
            {
                return null;
            }

            var k = dy / len;
            var dx = MathF.Abs(a.X - b.X) * k;

            if(a.X > b.X)
            {
                return a.X - dx;
            }

            return a.X + dx;
        }

        private Color Light(Color color, float a)
        {
            var k = 1.0f - (a < 0 ? 0 : (a > 2 ? 2 : a)) * 0.5f;
            return Color.FromArgb(
                color.A,
                (byte)((float)color.R * k),
                (byte)((float)color.G * k),
                (byte)((float)color.B * k));
        } 

        private (float, float) Area(Vector4F a, Vector4F b, Vector4F c)
        {
            var minY = MathF.Min(MathF.Min(a.Y, b.Y), c.Y);
            var maxY = MathF.Max(MathF.Max(a.Y, b.Y), c.Y);

            return (minY, maxY);
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);

            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
        }

        private Matrix4x4F Invert(Matrix4x4F m)
        {
            var nm = new System.Numerics.Matrix4x4(
                m.A11, m.A12, m.A13, m.A14,
                m.A21, m.A22, m.A23, m.A24,
                m.A31, m.A32, m.A33, m.A34,
                m.A41, m.A42, m.A43, m.A44
            );

            System.Numerics.Matrix4x4.Invert(nm, out var inv);

            return new Matrix4x4F(inv);
        } 
    }
}
