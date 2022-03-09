using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Windows.Media.Color;

namespace GKProj2
{
    public class Reflector
    {
        public Color Color { get; }
        public Vector3D Position { get; }

        public Reflector(Color color, Vector3D position)
        {
            Color = color;
            Position = position;
        }

        public int H
        {
            get => (int)Position.Z;
            set => Position.Z = value;
        }
    }
}
