using System;
using System.Collections.Generic;

namespace GKProj2
{
    public abstract class Polygon
    {
        public  List<Vertex> Vertices = new List<Vertex>();
        public  Drawing.EdgeNode[] GetEdgeNodes()
        {
            var (bottom, top) = Utility.GetSpread(Vertices);
            int height = top - bottom + 1;
            Drawing.EdgeNode[] nodes = new Drawing.EdgeNode[height];

            int len = Vertices.Count;
            for (int i = 0; i < len; ++i)
            {
                var (minX, maxX) = Utility.GetMinMax((int)Vertices[i].X, (int)Vertices[(i + 1) % len].X);
                var (minY, maxY) = Utility.GetMinMax((int)Vertices[i].Y, (int)Vertices[(i + 1) % len].Y);

                double rm;
                if (Math.Abs(Vertices[i].X - (int)Vertices[(i + 1) % len].X) < 1 ||
                    Math.Abs(Vertices[i].Y - (int)Vertices[(i + 1) % len].Y) < 1)
                    rm = 0;
                else
                    rm = (Vertices[i].X - Vertices[(i + 1) % len].X) / (Vertices[i].Y - Vertices[(i + 1) % len].Y);

                int x = (int)Vertices[i].Y == maxY ? (int)Vertices[(i + 1) % len].X : (int)Vertices[i].X;

                nodes[minY - bottom] = new Drawing.EdgeNode(maxY, x, rm, nodes[minY - bottom]);
            }

            return nodes;
        }
    }
    public class Triangle : Polygon
    {
        public Triangle(Vertex v1, Vertex v2, Vertex v3)
        {
            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);
        }
    }
}
