using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Draw3D.Math3D;
using Windows.UI;
using System;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Linq;

namespace Draw3D
{
    public sealed partial class MainPage : Page
    {
        private float angle = 0.0f;
        private int pixelSize = 1;
        private float pixelOffset = 0.5f;

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

        Vector4F eye = new Vector4F(0, 2, -3, 0);
        Vector4F lightPos = new Vector4F(-2, 3, -5, 0);   

        Matrix4x4F viewMatrix;
        Matrix4x4F perspectiveMatrix;
        Matrix4x4F worldToScreenMatrix;
        Matrix4x4F mvp;
        Matrix4x4F mvpInverted;
        Matrix4x4F rotation;

        public MainPage()
        {
            this.InitializeComponent();
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);
            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
            Matrix4x4F.Invert(mvp, out mvpInverted);
        } 

        private void OnAnimatedDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var a = angle * MathF.PI / 180.0f;
            rotation = Matrix4x4F.Rotation(a, a, a);
            //rotation = Matrix4x4F.RotationY(a);

            using (var session = args.DrawingSession)
            {
                DrawIndicies(vertices, indicies, session);
            }

            angle += 0.2f;
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

            faceNormal = Vector4F.Transform(rotation, faceNormal);

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
                var vertices = new List<Vector4F>();

                pa = Vector4F.Transform(worldToScreenMatrix, pa);
                pb = Vector4F.Transform(worldToScreenMatrix, pb);
                pc = Vector4F.Transform(worldToScreenMatrix, pc);

                //var light = Vector4F.Angle(
                //     Vector4F.Transform(rotation, faceNormal),
                //     new Vector4F(0, 0, -1));

                //var color = Light(Colors.YellowGreen, light);

                // rasterize
                var (minY, maxY) = Area(pa, pb, pc);
                var size = (int)maxY - minY;                
                
                var step = (int)minY;
                while(step < maxY)
                {
                    vertices.Clear();
                    AddVertexByY(pa, pb, step, vertices);
                    AddVertexByY(pb, pc, step, vertices);
                    AddVertexByY(pc, pa, step, vertices);

                    vertices = vertices.OrderBy(x => x.X).ToList();
                    if (vertices.Count > 1)
                    {
                        var vertexA = vertices.First();
                        var vertexB = vertices.Last();

                        for (int i = (int)MathF.Floor(vertexA.X); i < (int)MathF.Floor(vertexB.X) + pixelSize; i += pixelSize)
                        {

                            var point = GetInterpolatedByXY(vertexA, vertexB, i + pixelOffset, step + pixelOffset);
                            if(point != null)
                            {
                                var worldPoint = Vector4F.Transform(mvpInverted, point);
                                var light = Vector4F.Angle(
                                     faceNormal,
                                     Func3D.Normalyze(lightPos - worldPoint));

                                var color = Light(Colors.HotPink, light);
                                session.FillRectangle(i, step, pixelSize, pixelSize, color);
                            }                            
                        }                       
                    }

                    step += pixelSize;
                }
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

        private void DrawLineEx(Vector4F a, Vector4F b, CanvasDrawingSession session, Color color) => session.DrawLine(a.X, a.Y,b.X, b.Y, color, 1.0f);

        private (float?,float?) XOL(Vector4F a, Vector4F b, float y)
        {
            var pointA = GetXByY(a, b, y);
            var pointB = GetXByY(a, b, y+1);

            return (pointA, pointB);
        }

        private void AddVertexByY(Vector4F a, Vector4F b, float y, List<Vector4F> list)
        {

            var pointA = GetInterpolatedByY(a, b, y, y + 0.5f);
            if(pointA != null)
            {
                list.Add(pointA);
            }
        }

        private Vector4F GetInterpolatedByY(Vector4F a, Vector4F b, float pos, float y)
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
            var dz = MathF.Abs(a.Z - b.Z) * k;

            var x = a.X > b.X ? a.X - dx : a.X + dx;
            var z = a.Z > b.Z ? a.Z - dz : a.Z + dz;

            return new Vector4F(x, pos, z, 1);
        }

        private Vector4F GetInterpolatedByXY(Vector4F a, Vector4F b, float x, float y)
        {
            if (x > MathF.Max(a.X, b.X) || x < MathF.Min(a.X, b.X))
                return null;

            var dx = MathF.Abs(a.X - x);
            var len = MathF.Abs(a.X - b.X);

            if (len == 0)
            {
                return null;
            }

            var k = dx / len;
            var dz = MathF.Abs(a.Z - b.Z) * k;
            var z = a.Z > b.Z ? a.Z - dz : a.Z + dz;

            return new Vector4F(x, y, z, 1);
        }

        private float? GetXByY(Vector4F a, Vector4F b, float y)
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

            if (a.X > b.X)
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
            var minY = MathF.Floor(MathF.Min(MathF.Min(a.Y, b.Y), c.Y));
            var maxY = MathF.Floor(MathF.Max(MathF.Max(a.Y, b.Y), c.Y));

            return (minY, maxY);
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);

            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
            Matrix4x4F.Invert(mvp, out mvpInverted);
        }
    }
}
