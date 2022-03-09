using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Color = System.Windows.Media.Color;
using System.Windows.Media;


namespace GKProj2
{
    public class MainApp
    {
        private readonly DirectBitmap _bmp;
        private Hemisphere _sphere;
        private Vertex _selectedVertex;
        private readonly AppDependencies _appVariables;
        private readonly LightSpot _lightSpot;
        private DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Normal);

        private Reflector[] reflectors = new Reflector[3];

        public MainApp(AppDependencies appVariables, Canvas canvas)
        {
            reflectors[0] = new Reflector(Colors.Red, new Vector3D(0, AppDependencies.CanvasSize.Y, appVariables.H));
            reflectors[1] = new Reflector(Colors.Green, new Vector3D(AppDependencies.CanvasSize.X, AppDependencies.CanvasSize.Y, appVariables.H));
            reflectors[2] = new Reflector(Colors.Blue, new Vector3D(AppDependencies.CanvasSize.X/2, -AppDependencies.CanvasSize.Y/2.5, appVariables.H));

            _appVariables = appVariables;
            _appVariables.NormalMap = new DirectBitmap(new BitmapImage(new Uri(_appVariables.DefaultNormalPath)));
            _appVariables.Texture = new DirectBitmap(new BitmapImage(new Uri(_appVariables.DefaultTexturePath)));
            GenerateNewSphere();
            _bmp = new DirectBitmap(AppDependencies.CanvasSize);
            canvas.Children.Add(_bmp.GetDrawable());
            _lightSpot = new LightSpot(AppDependencies.CanvasSize, appVariables.HemisphereCenter);
            SetZofLight((int)(AppDependencies.CanvasSize.X / 2 * 1.5));
            Render();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            _timer.Tick += Animate;
            _timer.Start();
        }
        public void SetZofLight(int Z)
        {
            _lightSpot.Z = Z;
        }

        public void StartAnimation()
        {
            _timer.Start();
        }
        public void StopAnimation()
        {
            _timer.Stop();
        }

        public void Animate(object? sender, EventArgs e)
        {
            _lightSpot.UpdatePosition();
            Render();
        }

        public void CanvasLeftClicked(Point whereClicked)
        {
            _selectedVertex = _sphere.FindSelectedVertex(whereClicked);
        }
        public void MouseMovedTo(Point whereClicked)
        {
            if (_selectedVertex != null)
            {
                double z =Math.Sqrt(_appVariables.HemisphereR*_appVariables.HemisphereR - (Math.Pow(_sphere.HemisphereCenter.X - whereClicked.X, 2) + Math.Pow(_sphere.HemisphereCenter.Y - whereClicked.Y, 2)));
                if (double.IsNaN(z))
                    z = 0;
                _selectedVertex.MoveTo(new Vector3D(whereClicked.X, whereClicked.Y, z));
                Render();
            }
        }
        public void MouseReleased()
        {
            _selectedVertex = null;
        }

        public void GenerateNewSphere()
        {
            _sphere = new Hemisphere(_appVariables.TriangulationAccuracy, _appVariables.HemisphereCenter, _appVariables.HemisphereR);
        }

        public void Render()
        {
            _bmp.Lock();
            _bmp.Clear();
            Drawing.DrawHemisphere(_sphere, _lightSpot, reflectors, _bmp, _appVariables);
            _bmp.UpdateBitmap();
            _bmp.UnLock();
        }

        public void UpdateReflectors(int H)
        {
            for (int i = 0; i < 3; ++i)
                reflectors[i].H = H;
        }

    }
}
