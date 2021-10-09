using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Draw3D.Math3D;
using Windows.UI;
using System;

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

        Vector4F eye = new Vector4F(0, 2, -5, 0);
        Matrix4x4F viewMatrix;
        Matrix4x4F perspectiveMatrix;
        Matrix4x4F worldToScreenMatrix;
        Matrix4x4F mvp;

        public MainPage()
        {
            this.InitializeComponent();
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);

            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
        } 

        private void OnAnimatedDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var a = angle * MathF.PI / 180.0f;
            var rotation = Matrix4x4F.Rotation(a, a, a);


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

            angle += 1.0f;
            if (angle > 360.0f)
            {
                angle -= 360.0f;
            }            
        }

        private void OnSizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            viewMatrix = Matrix4x4F.LookAt(eye, new Vector4F(0, 0, 0, 0), new Vector4F(0, 1, 0, 0));
            perspectiveMatrix = Matrix4x4F.Perspective(40, 1, 1, 10);
            worldToScreenMatrix = Matrix4x4F.WorldToScreen(Canvas3D.Size);

            mvp = Matrix4x4F.Multiply(Matrix4x4F.Multiply(viewMatrix, perspectiveMatrix), worldToScreenMatrix);
        }
    }
}
