using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;
using Color = System.Windows.Media.Color;
using Image = System.Windows.Controls.Image;

namespace GKProj2
{
    public unsafe class DirectBitmap
    {
        private readonly WriteableBitmap _bmp;
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;
        private readonly Byte[] _internalArray;
        private readonly int _px_c;

        public DirectBitmap(Vector3D size)
        {
            _width = (int)size.X + 1;
            _height = (int)size.Y + 1;
            _bmp = new WriteableBitmap(_width, _height, 96, 96, PixelFormats.Bgra32, null);
            _stride = _bmp.BackBufferStride;
            _internalArray = new byte[_height * _stride * 4];
            _px_c = _bmp.Format.BitsPerPixel / 8;
        }
        public DirectBitmap(BitmapImage img)
        {
            _bmp = new WriteableBitmap(img);
            _width = (int)img.Width;
            _height = (int)img.Height;
            _stride = _bmp.BackBufferStride;
            _internalArray = new byte[_height * _stride * 4];
            _px_c = _bmp.Format.BitsPerPixel / 8;
            _bmp.Lock();
            for (int y = 0; y < _height; ++y)
                for (int x = 0; x < _width; ++x)
                {
                    Byte* px = (Byte*)_bmp.BackBuffer + _stride * y + _px_c * x;
                    _internalArray[y * _stride + _px_c * x + 0] = px[0];
                    _internalArray[y * _stride + _px_c * x + 1] = px[1];
                    _internalArray[y * _stride + _px_c * x + 2] = px[2];
                    _internalArray[y * _stride + _px_c * x + 3] = px[3];
                }
            _bmp.Unlock();
        }
        public Image GetDrawable() => new() { Source = _bmp };
        public void PutPixel(int x, int y, Color color)
        {
            if (x >= _width || y >= _height)
                return;
            
            _internalArray[y * _stride + _px_c * x + 0] = color.B;
            _internalArray[y * _stride + _px_c * x + 1] = color.G;
            _internalArray[y * _stride + _px_c * x + 2] = color.R;
            _internalArray[y * _stride + _px_c * x + 3] = color.A;
        }
        public Color GetPixel(int x, int y)
        {
            int xp = x % _width;
            int yp = y % _height;

            return Color.FromArgb(_internalArray[yp * _stride + _px_c * xp + 3], _internalArray[yp * _stride + _px_c * xp + 2], _internalArray[yp * _stride + _px_c * xp + 1], _internalArray[yp * _stride + _px_c * xp]);
        }
        public void Lock()
        {
            _bmp.Lock();
        }
        public void UnLock()
        {
            _bmp.Unlock();
        }

        public void UpdateBitmap()
        {
            _bmp.WritePixels(new Int32Rect(0, 0, _width, _height), _internalArray, _stride, 0);
            _bmp.AddDirtyRect(new Int32Rect(0, 0, _bmp.PixelWidth, _bmp.PixelHeight));
        }
        public void Clear()
        {
            Array.Clear(_internalArray, 0, _height * _stride * 4);
        }
    }
}
