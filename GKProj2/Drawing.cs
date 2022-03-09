using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace GKProj2
{
    public static class Drawing
    {
        public class EdgeNode
        {
            public double YMax { get; }
            public double X { get; set; }
            public double Rm { get; }
            public EdgeNode Next { get; set; }

            public EdgeNode(double ymax, double xmin, double rM, EdgeNode next = null)
            {
                YMax = ymax;
                X = xmin;
                Rm = rM;
                Next = next;
            }
        }

        public static void FillPolygon(Polygon poly, LightSpot ls, Reflector[] reflectors, DirectBitmap bmp, AppDependencies appModifiers)
        {
            var (bottom, top) = Utility.GetSpread(poly.Vertices);
            EdgeNode[] ET = poly.GetEdgeNodes();
            EdgeNode AET = ET[0];
            EdgeNode p;

            Color v1, v2, v3;

            if (appModifiers.ShadingType == ShadingType.Gouraud)
            {
                v1 = GetPhongColor(poly, ls, reflectors, (int)poly.Vertices[0].X, (int)poly.Vertices[0].Y, appModifiers);
                v2 = GetPhongColor(poly, ls, reflectors, (int)poly.Vertices[1].X, (int)poly.Vertices[1].Y, appModifiers);
                v3 = GetPhongColor(poly, ls, reflectors, (int)poly.Vertices[2].X, (int)poly.Vertices[2].Y, appModifiers);
            }

            for (int y = bottom + 1; y < top + 1; ++y)
            {
                //sorting
                AET = Utility.EdgeListBucketSortForX(AET);
                p = AET;

                //filling
                PaintPixel(poly, ls, reflectors, bmp, appModifiers, (int)p.X, y, v1, v2, v3);
                while (p != null && p.Next != null)
                {
                    if (Math.Abs(p.X - p.Next.X) < 0.8)
                    {
                        p = p.Next;
                        continue;
                    }

                    for (int x = (int)Math.Floor(p.X); x < (int)Math.Ceiling(p.Next.X); ++x)
                        PaintPixel(poly, ls, reflectors, bmp, appModifiers, x, y, v1, v2, v3);

                    p = p.Next.Next;
                }

                //removing
                Drawing.EdgeNode prev = AET;
                p = AET.Next;
                while (p != null)
                {
                    if((int)p.YMax < y)
                        prev.Next = p.Next;
                    p = p.Next;
                    prev = prev.Next;
                }
                if ((int)AET.YMax < y)
                    AET = AET.Next;

                //updating
                p = AET;
                while (p != null)
                {
                    p.X += p.Rm;
                    p = p.Next;
                }

                //adding
                p = AET;
                while (p.Next != null)
                    p = p.Next;
                p.Next = ET[y - bottom];
            }
        }

        public static void PaintPixel(Polygon poly, LightSpot ls, Reflector[] reflectors, DirectBitmap bmp, AppDependencies appModifiers, int x, int y, Color v1, Color v2, Color v3)
        {
            if (appModifiers.ShadingType == ShadingType.Phong)
                bmp.PutPixel(x, y, GetPhongColor(poly, ls, reflectors, x, y, appModifiers));
            else
                bmp.PutPixel(x, y, GetInterpolatedColor(poly, v1, v2, v3, x, y));
        }

        public static Color GetPhongColor(Polygon poly, LightSpot ls, Reflector[] reflectors, int x, int y, AppDependencies appModifiers)
        {
            Vector3D position = Utility.GetZFromInsideTriangle((Triangle)poly, x, y);
            Vector3D lightPosition = ls.Position;
            int textureX = x - ((int)appModifiers.HemisphereCenter.X - appModifiers.HemisphereR);
            int textureY = y - ((int)appModifiers.HemisphereCenter.Y - appModifiers.HemisphereR);

            Vector3D normalFromSphere = Utility.GetNormalFromSphere(position, appModifiers.HemisphereCenter);
            Vector3D Il = Utility.ColorToVector(appModifiers.LightColor);
            Vector3D V = new Vector3D(0, 0, 1);
            Vector3D L = Vector3D.NormalizePoint(lightPosition - position);
            Vector3D N, Io;
            if (appModifiers.NormalVectorType == NormalVectorType.Default || appModifiers.NormalMap == null)
                N = normalFromSphere;
            else
            {
                Vector3D B;
                if (normalFromSphere.X == 0 && normalFromSphere.Y == 0 && (int)Math.Round(normalFromSphere.Z) == 1)
                    B = new Vector3D(0, 1, 0);
                else
                    B = Vector3D.NormalizePoint(Vector3D.Cross(normalFromSphere, new Vector3D(0, 0, 1)));
                Vector3D T = Vector3D.NormalizePoint(Vector3D.Cross(B, normalFromSphere));
                Vector3D normalFromMap = Utility.ColorToNormalVector(appModifiers.NormalMap.GetPixel(textureX, textureY));

                Vector3D M = new Vector3D(T.X * normalFromMap.X + B.X * normalFromMap.Y + normalFromSphere.X * normalFromMap.Z,
                                          T.Y * normalFromMap.X + B.Y * normalFromMap.Y + normalFromSphere.Y * normalFromMap.Z, 
                                          T.Z * normalFromMap.X + B.Z * normalFromMap.Y + normalFromSphere.Z * normalFromMap.Z);
                N = Vector3D.NormalizePoint(normalFromSphere * appModifiers.Knvm + M * (1 - appModifiers.Knvm));
            }

            if (appModifiers.ColorType == ObjectColorType.FromSelectedColor || appModifiers.Texture == null)
                Io = Utility.ColorToVector(appModifiers.SphereColor);
            else
                Io = Utility.ColorToVector(appModifiers.Texture.GetPixel(textureX, textureY));

            double kd = appModifiers.Kd;
            double ks = appModifiers.Ks;
            double m = appModifiers.M;

            Vector3D R = N * Vector3D.Dot(N, L) * 2 - L;
            Vector3D I = new Vector3D(0, 0, 0);
            if(appModifiers.DrawSpiralLight)
                I = Il * Io * kd * Math.Max(Vector3D.Dot(N, L), 0) + Il * Io * ks * Math.Pow(Math.Max(Vector3D.Dot(V, R), 0), m);

            for (int i = 0; i != 3; ++i)
            {
                Vector3D Vp = Vector3D.NormalizePoint(reflectors[i].Position - position);
                Vector3D Vr = Vector3D.NormalizePoint(reflectors[i].Position - new Vector3D(appModifiers.HemisphereCenter.X, appModifiers.HemisphereCenter.Y, appModifiers.HemisphereR));
                Vector3D Ir = Utility.ColorToVector(reflectors[i].Color) * Math.Pow(Math.Max(Vector3D.Dot(Vr, Vp),0), appModifiers.Mr);

                Vector3D Rr = N * Vector3D.Dot(N, Vp) * 2 - Vp;
                I += Ir * Io * kd * Math.Max(Vector3D.Dot(N, Vp), 0) +
                         Ir * Io * ks * Math.Pow(Math.Max(Vector3D.Dot(V, Rr), 0), m);
            }

            return Utility.VectorToColor(I);
        }
        public static Color GetInterpolatedColor(Polygon poly, Color c1, Color c2, Color c3, int x, int y)
        {
            Vector3D a = new Vector3D(poly.Vertices[0].X, poly.Vertices[0].Y);
            Vector3D b = new Vector3D(poly.Vertices[1].X, poly.Vertices[1].Y);
            Vector3D c = new Vector3D(poly.Vertices[2].X, poly.Vertices[2].Y);
            Vector3D p = new Vector3D(x, y);

            Vector3D v0 = b - a;
            Vector3D v1 = c - a;
            Vector3D v2 = p - a;

            double d00 = Vector3D.Dot(v0, v0);
            double d01 = Vector3D.Dot(v0, v1);
            double d11 = Vector3D.Dot(v1, v1);
            double d20 = Vector3D.Dot(v2, v0);
            double d21 = Vector3D.Dot(v2, v1);
            double denom = d00 * d11 - d01 * d01;

            double W3 = (d11 * d20 - d01 * d21) / denom;
            double W2 = (d00 * d21 - d01 * d20) / denom;
            double W1 = 1 - W2 - W3;

            Vector3D cv1 = Utility.ColorToVector(c1);
            Vector3D cv2 = Utility.ColorToVector(c2);
            Vector3D cv3 = Utility.ColorToVector(c3);

            Vector3D final = cv1 * W1 + cv2 * W2 + cv3 * W3;

            return Utility.VectorToColor(final);
        }

        public static void DrawHemisphere(Hemisphere sphere, LightSpot ls, Reflector[] reflectors, DirectBitmap bmp, AppDependencies appModifiers)
        {
            if (appModifiers.ParallelDraw)
            {
                Parallel.ForEach(sphere.Triangles, triangle => { FillPolygon(triangle, ls, reflectors, bmp, appModifiers); });
                if (appModifiers.DrawMesh)
                {
                    Parallel.ForEach(sphere.Triangles, triangle =>
                    {
                        for (int i = 0; i < 3; ++i)
                            DrawLine(bmp, triangle.Vertices[i], triangle.Vertices[(i + 1) % 3], Colors.White);
                    });
                }
            }
            else
            {
                foreach (var triangle in sphere.Triangles)
                    FillPolygon(triangle, ls, reflectors, bmp, appModifiers);

                if (appModifiers.DrawMesh)
                {
                    foreach (var triangle in sphere.Triangles)
                        for (int i = 0; i < 3; ++i)
                            DrawLine(bmp, triangle.Vertices[i], triangle.Vertices[(i + 1) % 3], Colors.White);
                }
            }
        }

        public static void DrawLine(DirectBitmap bmp, Vertex v1, Vertex v2, Color color)
        {
            var tmpv1 = (X: (int)v1.X, Y: (int)v1.Y);
            var tmpv2 = (X: (int)v2.X, Y: (int)v2.Y);
            int dx = Math.Abs(tmpv2.X - tmpv1.X);
            int sx = v1.X < v2.X ? 1 : -1;
            int dy = -Math.Abs(tmpv2.Y - tmpv1.Y);
            int sy = v1.Y < v2.Y ? 1 : -1;
            int err = dx + dy;
            while (true)
            {
                bmp.PutPixel(tmpv1.X, tmpv1.Y, color);
                if (tmpv1.X == tmpv2.X && tmpv1.Y == tmpv2.Y) break;
                int e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    tmpv1.X += sx;
                }
                if (e2 <= dx)
                {
                    err += dx;
                    tmpv1.Y += sy;
                }
            }
        }

    }
}
