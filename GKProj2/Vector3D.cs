using System;
using System.Windows;

namespace GKProj2
{
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x, double y, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public Vector3D(Point vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = 0;
        }
        public Vector3D(Vertex v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public static Vector3D operator *(Vector3D v1, Vector3D v2)
        {
            return new(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static Vector3D operator *(Vector3D v1, double al)
        {
            return new(v1.X * al, v1.Y * al, v1.Z * al);
        }
        public static Vector3D operator /(Vector3D v1, double al)
        {
            return new(v1.X / al, v1.Y / al, v1.Z / al);
        }
        public static Vector3D Cross(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
        }
        public static double Dot(Vector3D v1, Vector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        public static Vector3D NormalizePoint(Vector3D v)
        {
            float m = 1 / (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            return new(v.X * m, v.Y * m, v.Z * m);
        }
        public static Vector3D Floor(Vector3D v)
        {
            return new(Math.Floor(v.X), Math.Floor(v.Y), Math.Floor(v.Z));
        }
    }
}
