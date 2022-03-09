using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace GKProj2
{
    public static class Utility
    {
        private const double Eps = 1E-16;
        public static (int min, int max) GetMinMax(int a, int b)
        {
            if (a > b)
                return (b, a);
            return (a, b);
        }
        public static (int bottom, int top) GetSpread(List<Vertex> Vertices)
        {
            int bottom = Int32.MaxValue;
            int top = Int32.MinValue;

            foreach (var v in Vertices)
            {
                if (v.Y < bottom)
                    bottom = (int)v.Y;

                if (v.Y > top)
                    top = (int)v.Y;
            }

            return (bottom, top);
        }
        public static Drawing.EdgeNode EdgeListBucketSortForX(Drawing.EdgeNode root)
        {
            if (root == null) return null;
            int bottom = Int32.MaxValue;
            int top = Int32.MinValue;
            Drawing.EdgeNode p = root;
            while (p != null)
            {
                if (p.X < bottom) bottom = (int)p.X;
                if (p.X > top) top = (int)p.X;
                p = p.Next;
            }

            p = root;
            Drawing.EdgeNode next = root.Next;
            Drawing.EdgeNode[] buckets = new Drawing.EdgeNode[top - bottom + 1];

            while (p != null)
            {
                p.Next = null;
                if (buckets[(int)p.X - bottom] == null)
                    buckets[(int)p.X - bottom] = p;
                else
                {
                    p.Next = buckets[(int)p.X - bottom];
                    buckets[(int)p.X - bottom] = p;
                }

                p = next;
                if (next != null)
                    next = next.Next;
            }

            Drawing.EdgeNode root2 = buckets[0];
            p = root2;
            for (int i = 1; i < top - bottom + 1; ++i)
            {
                if (buckets[i] != null)
                {
                    while (p.Next != null)
                        p = p.Next;

                    p.Next = buckets[i];
                }
            }

            return root2;
        }
        public static double Distance2D(Vector3D v1, Vector3D v2)
            => Math.Sqrt(Math.Pow(v1.X - v2.X, 2) + Math.Pow(v1.Y - v2.Y, 2));

        public static Vector3D GetZFromInsideTriangle(Triangle t, double x, double y)
        {
            var x1 = t.Vertices[0].X;
            var x2 = t.Vertices[1].X;
            var x3 = t.Vertices[2].X;
            var y1 = t.Vertices[0].Y;
            var y2 = t.Vertices[1].Y;
            var y3 = t.Vertices[2].Y;
            var z1 = t.Vertices[0].Z;
            var z2 = t.Vertices[1].Z;
            var z3 = t.Vertices[2].Z;

            var z = z1 +
                (((x2 - x1) * (z3 - z1) - (x3 - x1) * (z2 - z1)) / ((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1))) *
                (y - y1) - (x - x1) * (((y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1)) /
                                       ((x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1)));

            return new Vector3D(x, y, z);
        }
        
        public static Vector3D ColorToVector(Color color)
            => new (color.R / 255.0, color.G / 255.0, color.B / 255.0);

        public static Color VectorToColor(Vector3D color)
        {
            double x = double.IsNaN(color.X) ? 0 : color.X;
            double y = double.IsNaN(color.Y) ? 0 : color.Y;
            double z = double.IsNaN(color.Z) ? 0 : color.Z;

            return new()
            {
                A = 255,
                R = Convert.ToByte(Math.Max(Math.Min(x, 1.0), 0) * 255),
                G = Convert.ToByte(Math.Max(Math.Min(y, 1.0), 0) * 255),
                B = Convert.ToByte(Math.Max(Math.Min(z, 1.0), 0) * 255)
            };
        }

        public static Vector3D ColorToNormalVector(Color color)
            => new (color.R * 2.0 / 255 - 1, color.G * 2.0 / 255 - 1, color.B * 2.0 / 255 - 1);

        public static Vector3D GetNormalFromSphere(Vector3D pointOnSphere, Vector3D sphereCenter)
            => Vector3D.NormalizePoint(pointOnSphere - sphereCenter);
    }
}
