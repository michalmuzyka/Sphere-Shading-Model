namespace GKProj2
{
    public class Vertex
    {
        public double X { private set; get; }
        public double Y { private set; get; }
        public double Z { private set; get; }

        public Vertex(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public void MoveTo(Vector3D v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }
    }
}
