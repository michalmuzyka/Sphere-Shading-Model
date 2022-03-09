using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Win32;
using Color = System.Windows.Media.Color;

namespace GKProj2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainApp _app;
        private readonly AppDependencies _appVariables = new AppDependencies();
        private bool _animate = true;
        public MainWindow()
        {
            InitializeComponent();
            _app = new MainApp(_appVariables, mainCanvas);
            SphereColor.Fill = new SolidColorBrush(_appVariables.SphereColor);
            LightColor.Fill = new SolidColorBrush(_appVariables.LightColor);
        }

        private void mainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _app?.CanvasLeftClicked(Mouse.GetPosition(mainCanvas));
        }
        private void mainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            _app?.MouseMovedTo(Mouse.GetPosition(mainCanvas));
        }
        private void mainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _app?.MouseReleased();
        }
        private void triangulationAccuracy_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.TriangulationAccuracy = (int)e.NewValue;
            _app?.GenerateNewSphere();
            _app?.Render();
        }

        private void hemisphereR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.HemisphereR = (int)e.NewValue;
            _app?.GenerateNewSphere();
            _app?.Render();
        }

        private void MeshCheckbox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (meshCheckbox.IsChecked != null) 
                _appVariables.DrawMesh = meshCheckbox.IsChecked.Value;
            _app?.Render();
        }

        private void ParallelCheckbox_OnChecked(object sender, RoutedEventArgs e)
        {
            if (parallelCheckbox.IsChecked != null) 
                _appVariables.ParallelDraw = parallelCheckbox.IsChecked.Value;
        }

        private void RadioFromSelectedColor_Clicked(object sender, RoutedEventArgs e)
        {
            _appVariables.ColorType = ObjectColorType.FromSelectedColor;
            _app?.Render();
        }
        private void ButtonSelectColor_Clicked(object sender, RoutedEventArgs e)
        {
            RadioFromTexture.IsChecked = false;
            RadioFromSelectedColor.IsChecked = true;
            _appVariables.ColorType = ObjectColorType.FromSelectedColor;
            ColorPicker.ColorPicker colorPicker = new ColorPicker.ColorPicker();
            colorPicker.PickedColor = _appVariables.SphereColor;
            if (colorPicker.ShowDialog().HasValue)
            {
                _appVariables.SphereColor = colorPicker.PickedColor;
                SphereColor.Fill = new SolidColorBrush(_appVariables.SphereColor);
            }

            _app?.Render();
        }
        private void RadioFromTexture_Clicked(object sender, RoutedEventArgs e)
        {
            _appVariables.ColorType = ObjectColorType.FromTexture;
            if(_appVariables.Texture == null)
                LoadTexture();
            _app?.Render();
        }
        private void ButtonSelectTexture_Clicked(object sender, RoutedEventArgs e)
        {
            RadioFromTexture.IsChecked = true;
            RadioFromSelectedColor.IsChecked = false;
            _appVariables.ColorType = ObjectColorType.FromTexture;
            LoadTexture();
            _app?.Render();
        }

        private void LoadTexture()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;|All files (*.*)|*.*";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\Textures";
            var res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
                _appVariables.Texture = new DirectBitmap(new BitmapImage(new Uri(ofd.FileName)));
            else if (_appVariables.Texture == null)
            {
                _appVariables.ColorType = ObjectColorType.FromSelectedColor;
                RadioFromTexture.IsChecked = false;
                RadioFromSelectedColor.IsChecked = true;
            }
        }

        private void ButtonSelectLightColor_Clicked(object sender, RoutedEventArgs e)
        {
            ColorPicker.ColorPicker colorPicker = new ColorPicker.ColorPicker();
            colorPicker.PickedColor = _appVariables.LightColor;
            if (colorPicker.ShowDialog().HasValue)
            {
                _appVariables.LightColor = colorPicker.PickedColor;
                LightColor.Fill = new SolidColorBrush(_appVariables.LightColor);
            }

            _app?.Render();
        }

        private void RadioDefaultNormals_Clicked(object sender, RoutedEventArgs e)
        {
            _appVariables.NormalVectorType = NormalVectorType.Default;
            _app?.Render();
        }
        private void RadioNormalsFromTexture_Clicked(object sender, RoutedEventArgs e)
        {
            _appVariables.NormalVectorType = NormalVectorType.FromTexture;
            if(_appVariables.NormalMap == null)
                LoadNormalMap();
            _app?.Render();
        }

        private void LoadNormalMap()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;|All files (*.*)|*.*";
            ofd.InitialDirectory = Directory.GetCurrentDirectory() + "\\NormalMaps";
            var res = ofd.ShowDialog();
            if (res.HasValue && res.Value)
                _appVariables.NormalMap = new DirectBitmap(new BitmapImage(new Uri(ofd.FileName)));
            else if(_appVariables.NormalMap == null)
            {
                _appVariables.NormalVectorType = NormalVectorType.Default;
                RadioNormalsFromTexture.IsChecked = false;
                RadioDefaultNormals.IsChecked = true;
            }
        }

        private void spiralLightCheckbox_OnChecked(object sender, RoutedEventArgs e)
        {
            _appVariables.DrawSpiralLight = !_appVariables.DrawSpiralLight;
            _app?.Render();
        }

        private void ButtonSelectNormalMap_Clicked(object sender, RoutedEventArgs e)
        {
            RadioDefaultNormals.IsChecked = false;
            RadioNormalsFromTexture.IsChecked = true;
            _appVariables.NormalVectorType = NormalVectorType.FromTexture;
             LoadNormalMap();
            _app?.Render();
        }

        private void ZCordSlider_OnValueChanged_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _app?.SetZofLight((int)e.NewValue);
            _app?.Render();
        }

        private void ButtonStartStopAnimation_Clicked(object sender, RoutedEventArgs e)
        {
            _animate = !_animate;
            if (!_animate)
            {
                _app.StopAnimation();
                StartStopAnimation.Content = "Wznów animację";
            }
            else
            {
                _app.StartAnimation();
                StartStopAnimation.Content = "Zatrzymaj animację";
            }
        }
        private void RadioPhong_Clicked(object sender, RoutedEventArgs e)
        {
            _appVariables.ShadingType = ShadingType.Phong;
            _app?.Render();
        }
        private void RadioGouraud_Clicked(object sender, RoutedEventArgs e)
        {
            _appVariables.ShadingType = ShadingType.Gouraud;
            _app?.Render();
        }

        private void SliderKs_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.Ks = e.NewValue;
            _app?.Render();
        }

        private void SliderKd_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.Kd = e.NewValue;
            _app?.Render();
        }

        private void SliderKnvm_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.Knvm = e.NewValue;
            _app?.Render();
        }

        private void SliderM_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.M = (int)e.NewValue;
            _app?.Render();
        }

        private void SliderH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.H = (int)e.NewValue;
            _app?.UpdateReflectors(_appVariables.H);
            _app?.Render();
        }

        private void SliderMr_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _appVariables.Mr = (int)e.NewValue;
            _app?.Render();
        }
    }
}
