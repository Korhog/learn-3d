using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Draw3D.Math3D;
using Windows.UI;
using System;
using Microsoft.Graphics.Canvas;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Draw3D
{
    public sealed partial class MainPage : Page
    {
        private float angle = 0.0f;
        private int pixelSize = 1;
        private float pixelOffset = 0.5f;

        private CanvasBitmap cb;

        private readonly Dictionary<(int, int), Color> colorCache = new Dictionary<(int, int), Color>();

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
            new Vertex(new Vector4F( 1, -1, -1, 1)) // 7 A D F
        };

        Vertex[] cube = new Vertex[]
        {
            // front side
            new Vertex(new Vector4F(-1,-1,-1, 1), Func3D.Normalyze(new Vector4F(-1,-1,-2, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F(-1, 1,-1, 1), Func3D.Normalyze(new Vector4F(-1, 1,-2, 1)), new Vector2F(0,0)),
            new Vertex(new Vector4F( 1, 1,-1, 1), Func3D.Normalyze(new Vector4F( 1, 1,-2, 1)), new Vector2F(1,0)),

            new Vertex(new Vector4F(-1,-1,-1, 1), Func3D.Normalyze(new Vector4F(-1,-1,-2, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F( 1, 1,-1, 1), Func3D.Normalyze(new Vector4F( 1, 1,-2, 1)), new Vector2F(1,0)),
            new Vertex(new Vector4F( 1,-1,-1, 1), Func3D.Normalyze(new Vector4F( 1,-1,-2, 1)), new Vector2F(1,1)),
            
            // back side
            new Vertex(new Vector4F( 1,-1, 1, 1), Func3D.Normalyze(new Vector4F( 1,-1, 2, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F( 1, 1, 1, 1), Func3D.Normalyze(new Vector4F( 1, 1, 2, 1)), new Vector2F(0,0)),
            new Vertex(new Vector4F(-1, 1, 1, 1), Func3D.Normalyze(new Vector4F(-1, 1, 2, 1)), new Vector2F(1,0)),

            new Vertex(new Vector4F( 1,-1, 1, 1), Func3D.Normalyze(new Vector4F( 1,-1, 2, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F(-1, 1, 1, 1), Func3D.Normalyze(new Vector4F(-1, 1, 2, 1)), new Vector2F(1,0)),
            new Vertex(new Vector4F(-1,-1, 1, 1), Func3D.Normalyze(new Vector4F(-1,-1, 2, 1)), new Vector2F(1,1)),
            
            // right side
            new Vertex(new Vector4F( 1,-1,-1, 1), Func3D.Normalyze(new Vector4F( 2,-1,-1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F( 1, 1,-1, 1), Func3D.Normalyze(new Vector4F( 2, 1,-1, 1)), new Vector2F(0,0)),
            new Vertex(new Vector4F( 1, 1, 1, 1), Func3D.Normalyze(new Vector4F( 2, 1, 1, 1)), new Vector2F(1,0)),

            new Vertex(new Vector4F( 1,-1,-1, 1), Func3D.Normalyze(new Vector4F( 2,-1,-1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F( 1, 1, 1, 1), Func3D.Normalyze(new Vector4F( 2, 1, 1, 1)), new Vector2F(1,0)),
            new Vertex(new Vector4F( 1,-1, 1, 1), Func3D.Normalyze(new Vector4F( 2,-1, 1, 1)), new Vector2F(1,1)),

            // left side
            new Vertex(new Vector4F(-1,-1, 1, 1), Func3D.Normalyze(new Vector4F(-2,-1, 1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F(-1, 1, 1, 1), Func3D.Normalyze(new Vector4F(-2, 1, 1, 1)), new Vector2F(0,0)),
            new Vertex(new Vector4F(-1, 1,-1, 1), Func3D.Normalyze(new Vector4F(-2, 1,-1, 1)), new Vector2F(1,0)),

            new Vertex(new Vector4F(-1,-1, 1, 1), Func3D.Normalyze(new Vector4F(-2,-1, 1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F(-1, 1,-1, 1), Func3D.Normalyze(new Vector4F(-2, 1,-1, 1)), new Vector2F(1,0)),
            new Vertex(new Vector4F(-1,-1,-1, 1), Func3D.Normalyze(new Vector4F(-2,-1,-1, 1)), new Vector2F(1,1)),

            // bottom side
            new Vertex(new Vector4F( 1,-1,-1, 1), Func3D.Normalyze(new Vector4F( 1,-2,-1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F( 1,-1, 1, 1), Func3D.Normalyze(new Vector4F( 1,-2, 1, 1)), new Vector2F(0,0)),
            new Vertex(new Vector4F(-1,-1, 1, 1), Func3D.Normalyze(new Vector4F(-1,-2, 1, 1)), new Vector2F(1,0)),

            new Vertex(new Vector4F( 1,-1,-1, 1), Func3D.Normalyze(new Vector4F( 1,-2,-1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F(-1,-1, 1, 1), Func3D.Normalyze(new Vector4F(-1,-2, 1, 1)), new Vector2F(1,0)),
            new Vertex(new Vector4F(-1,-1,-1, 1), Func3D.Normalyze(new Vector4F(-1,-2,-1, 1)), new Vector2F(1,1)),

             // top side
            new Vertex(new Vector4F(-1, 1,-1, 1), Func3D.Normalyze(new Vector4F(-1, 2,-1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F(-1, 1, 1, 1), Func3D.Normalyze(new Vector4F(-1, 2, 1, 1)), new Vector2F(0,0)),
            new Vertex(new Vector4F( 1, 1, 1, 1), Func3D.Normalyze(new Vector4F( 1, 2, 1, 1)), new Vector2F(1,0)),

            new Vertex(new Vector4F(-1, 1,-1, 1), Func3D.Normalyze(new Vector4F(-1, 2,-1, 1)), new Vector2F(0,1)),
            new Vertex(new Vector4F( 1, 1, 1, 1), Func3D.Normalyze(new Vector4F( 1, 2, 1, 1)), new Vector2F(1,0)),
            new Vertex(new Vector4F( 1, 1,-1, 1), Func3D.Normalyze(new Vector4F( 1, 2,-1, 1)), new Vector2F(1,1))
        };

        int[] cubeIndicies = new int[] {
             0, 1, 2, 3, 4, 5,
             6, 7, 8, 9,10,11,
            12,13,14,15,16,17,
            18,19,20,21,22,23,
            24,25,26,27,28,29,
            30,31,32,33,34,35
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

            Task.Run(async () =>
            {
                var device = CanvasDevice.GetSharedDevice();
                cb = await CanvasBitmap.LoadAsync(device, "texture.png");      
            });

            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);
            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
            Matrix4x4F.Invert(mvp, out mvpInverted);


        } 

        private void OnAnimatedDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            if(cb == null)
            {
                return;
            }

            var a = angle * MathF.PI / 180.0f;
            rotation = Matrix4x4F.Rotation(a, a, a);
            //rotation = Matrix4x4F.RotationY(0);

            using (var session = args.DrawingSession)
            {
                DrawIndicies(cube, cubeIndicies, session);
            }

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
            //if (true)           
            {
                var vertices = new List<Vertex>();

                var va = new Vertex(Vector4F.Transform(worldToScreenMatrix, pa), a.Normal, a.UV);
                var vb = new Vertex(Vector4F.Transform(worldToScreenMatrix, pb), b.Normal, b.UV);
                var vc = new Vertex(Vector4F.Transform(worldToScreenMatrix, pc), c.Normal, c.UV);

                //var light = Vector4F.Angle(
                //     Vector4F.Transform(rotation, faceNormal),
                //     new Vector4F(0, 0, -1));

                //var color = Light(Colors.YellowGreen, light);

                // rasterize
                var (minY, maxY) = Area(va.Position, vb.Position, vc.Position);
                var size = (int)maxY - minY;

                var step = (int)minY;
                while (step < maxY)
                {
                    vertices.Clear();
                    AddVertexByY(va, vb, step, vertices);
                    AddVertexByY(vb, vc, step, vertices);
                    AddVertexByY(vc, va, step, vertices);

                    vertices = vertices.OrderBy(x => x.Position.X).ToList();
                    if (vertices.Count > 1)
                    {
                        var vertexA = vertices.First();
                        var vertexB = vertices.Last();

                        for (int i = (int)MathF.Floor(vertexA.Position.X); i < (int)MathF.Floor(vertexB.Position.X) + pixelSize; i += pixelSize)
                        {

                            var point = GetInterpolatedByXY(vertexA, vertexB, i + pixelOffset, step + pixelOffset);
                            if (point != null)
                            {
                                var worldPoint = Vector4F.Transform(mvpInverted, point.Position);
                                var light = Vector4F.Angle(
                                     //Vector4F.Transform(rotation, point.Normal),
                                     faceNormal,
                                     Func3D.Normalyze(lightPos - worldPoint));

                                var color = GetFromTexture(point.UV.X, point.UV.Y, cb);

                                //var color = Color.FromArgb(
                                //    255,
                                //    (byte)(point.UV.X * 255f),
                                //    0,
                                //    (byte)(point.UV.Y * 255f));

                                //var color = Colors.Gray;

                                color = Light(color, light);
                                session.FillRectangle(i, step, pixelSize, pixelSize, color);
                            }
                        }
                    }

                    step += pixelSize;
                }

                //DrawLine(a, b, session, Colors.Red);
                //DrawLine(b, c, session, Colors.Red);
                //DrawLine(c, a, session, Colors.Red);
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

        private void AddVertexByY(Vertex a, Vertex b, float y, List<Vertex> list)
        {
            var pointA = GetInterpolatedByY(a, b, y, y + 0.5f);
            if(pointA != null)
            {
                list.Add(pointA);
            }
        }

        private Vertex GetInterpolatedByY(Vertex a, Vertex b, float pos, float y)
        {
            if (y > MathF.Max(a.Position.Y, b.Position.Y) || y < MathF.Min(a.Position.Y, b.Position.Y))
                return null;

            var dy = MathF.Abs(a.Position.Y - y);
            var len = MathF.Abs(a.Position.Y - b.Position.Y);

            if (len == 0)
            {
                return null;
            }

            var k = dy / len;

            // interpolate position
            var dx = MathF.Abs(a.Position.X - b.Position.X) * k;
            var dz = MathF.Abs(a.Position.Z - b.Position.Z) * k;

            var x = a.Position.X > b.Position.X ? a.Position.X - dx : a.Position.X + dx;
            var z = a.Position.Z > b.Position.Z ? a.Position.Z - dz : a.Position.Z + dz;

            var position = new Vector4F(x, pos, z, 1);

            // interpolate normal
            var dnx = MathF.Abs(a.Normal.X - b.Normal.X) * k;
            var dny = MathF.Abs(a.Normal.Y - b.Normal.Y) * k;
            var dnz = MathF.Abs(a.Normal.Z - b.Normal.Z) * k;

            var nx = a.Normal.X > b.Normal.X ? a.Normal.X - dnx : a.Normal.X + dnx;
            var ny = a.Normal.Y > b.Normal.Y ? a.Normal.Y - dny : a.Normal.Y + dny;
            var nz = a.Normal.Z > b.Normal.Z ? a.Normal.Z - dnz : a.Normal.Z + dnz;

            var normal = Func3D.Normalyze(new Vector4F(nx, ny, nz, 1));

            // interpolate uv
            var du = MathF.Abs(a.UV.X - b.UV.X) * k;
            var dv = MathF.Abs(a.UV.Y - b.UV.Y) * k;

            var u = a.UV.X > b.UV.X ? a.UV.X - du : a.UV.X + du;
            var v = a.UV.Y > b.UV.Y ? a.UV.Y - dv : a.UV.Y + dv;

            return new Vertex(position, normal, new Vector2F(u, v));
        }

        private Vertex GetInterpolatedByXY(Vertex a, Vertex b, float x, float y)
        {
            if (x > MathF.Max(a.Position.X, b.Position.X) || x < MathF.Min(a.Position.X, b.Position.X))
                return null;

            var dx = MathF.Abs(a.Position.X - x);
            var len = MathF.Abs(a.Position.X - b.Position.X);

            if (len == 0)
            {
                return null;
            }

            var k = dx / len;
            var positon = GetInterpolatedPositionByXY(a.Position, b.Position, k, x, y);


            // interpolate normal
            var dnx = MathF.Abs(a.Normal.X - b.Normal.X) * k;
            var dny = MathF.Abs(a.Normal.Y - b.Normal.Y) * k;
            var dnz = MathF.Abs(a.Normal.Z - b.Normal.Z) * k;

            var nx = a.Normal.X > b.Normal.X ? a.Normal.X - dnx : a.Normal.X + dnx;
            var ny = a.Normal.Y > b.Normal.Y ? a.Normal.Y - dny : a.Normal.Y + dny;
            var nz = a.Normal.Z > b.Normal.Z ? a.Normal.Z - dnz : a.Normal.Z + dnz;

            var normal = Func3D.Normalyze(new Vector4F(nx, ny, nz, 1));

            // interpolate uv
            var du = MathF.Abs(a.UV.X - b.UV.X) * k;
            var dv = MathF.Abs(a.UV.Y - b.UV.Y) * k;

            var u = a.UV.X > b.UV.X ? a.UV.X - du : a.UV.X + du;
            var v = a.UV.Y > b.UV.Y ? a.UV.Y - dv : a.UV.Y + dv;

            return new Vertex(positon, normal, new Vector2F(u,v));
        }

        private Vector4F GetInterpolatedPositionByXY(Vector4F a, Vector4F b, float k, float x, float y)
        {
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

        private Color GetFromTexture(float u, float v, CanvasBitmap texture)
        {
            var tx = (int)((texture.SizeInPixels.Width - 1) * u);
            

            var ty = (int)((texture.SizeInPixels.Height - 1) * v);

            if(!colorCache.ContainsKey((tx,ty)))
            {
                var color = texture.GetPixelColors((int)tx, (int)ty, 1, 1)[0];
                colorCache[(tx, ty)] = color;
                return color;
            }

            return colorCache[(tx, ty)];
        }
    }
}
