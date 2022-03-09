using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace GKProj2
{
    public class Hemisphere
    {
        private static readonly List<Vector3D> PyramidVertices = new List<Vector3D>()
        {
            new (0, 0, 1),
            new (1, 0, 0),
            new (0, 1, 0),
            new (-1, 0, 0),
            new (0, -1, 0)
        };

        private static readonly List<int[]> PyramidTriangleIndexes = new List<int[]>()
        {
            new []{0, 1, 2},
            new []{0, 4, 1},
            new []{0, 3, 4}, 
            new []{0, 2, 3},
        };

        private List<Vector3D> Vertices = new List<Vector3D>(PyramidVertices);
        private List<int[]> TriangleIndexes = new List<int[]>(PyramidTriangleIndexes);

        private readonly List<Vertex> _vert = new List<Vertex>();
        public readonly List<Triangle> Triangles = new List<Triangle>();
        public Vector3D HemisphereCenter { get; }
        public static readonly int VertexHitbox = 5;
        public Hemisphere(int accuracy, Vector3D hemisphereCenter, int R)
        {
            HemisphereCenter = hemisphereCenter;

            for(int i=0; i<accuracy; ++i)
                DivideEachTriangleIntoFour();

            for (int i = 0; i < Vertices.Count; ++i)
            {
                Vertices[i] *= R;
                Vertices[i] = Vector3D.Floor(Vertices[i] + HemisphereCenter);
            }

            foreach (var v in Vertices)
                _vert.Add(new Vertex(v.X, v.Y, v.Z));

            foreach (var triangle in TriangleIndexes)
                Triangles.Add(new Triangle(_vert[triangle[0]], _vert[triangle[1]], _vert[triangle[2]]));
        }

        public Vertex FindSelectedVertex(Point whereClicked)
        {
            foreach (var vertex in _vert)
                if (Utility.Distance2D(new Vector3D(vertex), new Vector3D(whereClicked)) < VertexHitbox)
                    return vertex;

            return null;
        }

        private void DivideEachTriangleIntoFour()
        {
            List<int[]> tmpTriangles = new List<int[]>();
            Dictionary<(int, int), int> lookupTable = new Dictionary<(int, int), int>();
            int[] midPoints = new int[3];

            foreach (var triangle in TriangleIndexes)
            {
                for (int i = 0; i < 3; ++i)
                    midPoints[i] = DivideEdge(lookupTable, triangle[i], triangle[(i + 1) % 3]);

                tmpTriangles.Add(new[] { triangle[0], midPoints[0], midPoints[2] });
                tmpTriangles.Add(new[] { triangle[1], midPoints[1], midPoints[0] });
                tmpTriangles.Add(new[] { triangle[2], midPoints[2], midPoints[1] });
                tmpTriangles.Add(new[] { midPoints[0], midPoints[1], midPoints[2] });
            }

            TriangleIndexes = tmpTriangles;
        }
        private int DivideEdge(Dictionary<(int, int), int> lookupTable, int v1Id, int v2Id)
        {
            var key = v1Id < v2Id ? (v1Id, v2Id) : (v2Id, v1Id);

            if (lookupTable.ContainsKey(key))
                return lookupTable[key];

            lookupTable.Add(key, Vertices.Count);

            Vertices.Add(Vector3D.NormalizePoint((Vertices[v1Id] + Vertices[v2Id]) / 2));
            return Vertices.Count - 1;
        }
    }
}
