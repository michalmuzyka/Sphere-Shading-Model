using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GKProj2.ColorPicker
{
    /// <summary>
    /// Logika interakcji dla klasy ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        public Color PickedColor
        {
            get => Color.FromArgb(255, Convert.ToByte(redSlider.Value), Convert.ToByte(greenSlider.Value), Convert.ToByte(blueSlider.Value));
            set
            {
                redSlider.Value = value.R;
                greenSlider.Value = value.G;
                blueSlider.Value = value.B;
            }
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
