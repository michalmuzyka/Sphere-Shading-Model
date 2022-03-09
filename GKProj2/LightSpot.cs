using System;

namespace GKProj2
{
    public class LightSpot
    {
        private Vector3D Center { get; }

        public Vector3D Position
        {
            get => new Vector3D(Center.X + _r * Math.Cos(_angle), Center.Y + _r * Math.Sin(_angle), Z);
        }

        private Vector3D Boundaries { get; }

        private double _angle = 0.0;
        private readonly double _angleDelta = 0.2;

        private double _r = 0.0;
        private double _rDelta = 10;
        
        public int Z { get; set; }

        public LightSpot(Vector3D boundaries, Vector3D center)
        {
            Center = center;
            Boundaries = boundaries;
        }

        public void UpdatePosition()
        {
            _angle += _angleDelta;
            if (_angle > 2 * Math.PI)
                _angle = 0.0;

            _r += _rDelta;
            if (_r <= 0 || _r >= Boundaries.X * 2)
                _rDelta = -_rDelta;
        }
    }
}
