using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Draw3D.Math3D;
using Windows.UI;
using Microsoft.Graphics.Canvas;

namespace Draw3D
{
    public sealed partial class MainPage : Page
    {
        Vector4F[] box = new Vector4F[8]
        {
            new Vector4F(-100,  100, -100, 1),
            new Vector4F(-100,  100,  100, 1),
            new Vector4F(-100, -100,  100, 1),
            new Vector4F(-100, -100, -100, 1),
            new Vector4F( 100,  100, -100, 1),
            new Vector4F( 100,  100,  100, 1),
            new Vector4F( 100, -100,  100, 1),
            new Vector4F( 100, -100, -100, 1)
        };

        Vector4F eye = new Vector4F(500, 300, -500, 0);
        Matrix4x4F viewMatrix;

        public MainPage()
        {
            this.InitializeComponent();
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));           
        }

        // Basic draw cyc
        private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args) 
        { 
            DrawGrid(sender, args.DrawingSession);

            var p0 = WorldToScreen(sender, new Vector2F());

            foreach (var p in box)
            {
                var pt = Func3D.Mul(viewMatrix, p);
                pt = Project(pt);
                var pw = WorldToScreen(sender, new Vector2F(pt.X, pt.Y));
                args.DrawingSession.FillCircle(pw.X, pw.Y, 2, Colors.Yellow);                
            }

            args.DrawingSession.FillCircle(p0.X, p0.Y, 2, Colors.Red);
        }

        Vector4F Project(Vector4F v)
        {
            var n = 500;
            var x = n * v.X / v.Z;
            var y = n * v.Y / v.Z;
            return new Vector4F(x, y, v.Z, v.W);
        }

        private Vector2F WorldToScreen(CanvasControl canvas, Vector2F p)
        {
            var center = Center(canvas);
            return new Vector2F(center.X + p.X, center.Y - p.Y);
        }

        private Vector2F Center(CanvasControl canvas) => new Vector2F(
                (float)(canvas.ActualWidth / 2),
                (float)(canvas.ActualHeight / 2)
            );

        private void DrawGrid(CanvasControl canvas, CanvasDrawingSession session) 
        {
            var center = Center(canvas);
            session.DrawLine(center.X, 0, center.X, (float)canvas.ActualHeight, Color.FromArgb(50, 0, 255, 0));
            session.DrawLine(0, center.Y, (float)canvas.ActualWidth, center.Y, Color.FromArgb(50, 0, 255, 0));
        }
    }
}
