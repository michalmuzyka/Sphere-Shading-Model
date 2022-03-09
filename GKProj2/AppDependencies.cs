using System.IO;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace GKProj2
{
    public enum ObjectColorType
    {
        FromTexture,
        FromSelectedColor
    }
    public enum ShadingType
    {
        Phong,
        Gouraud
    }
    public enum NormalVectorType
    {
        Default,
        FromTexture
    }

    public class AppDependencies
    {
        public static readonly Vector3D CanvasSize = new Vector3D(916, 916);
        public Vector3D HemisphereCenter = CanvasSize/2;
        public int HemisphereR = (int)CanvasSize.X / 5;
        public int TriangulationAccuracy = 3;
        public bool DrawMesh = false;
        public bool ParallelDraw = true;

        public ObjectColorType ColorType = ObjectColorType.FromSelectedColor;
        public DirectBitmap Texture = null;
        public Color SphereColor = Colors.WhiteSmoke;

        public string DefaultNormalPath = Directory.GetCurrentDirectory() + "\\NormalMaps\\Tiles_047_normal96.jpg";
        public string DefaultTexturePath = Directory.GetCurrentDirectory() + "\\Textures\\Tiles_047_basecolor96.jpg";

        public DirectBitmap NormalMap = null;
        public NormalVectorType NormalVectorType = NormalVectorType.Default;

        public Color LightColor = Colors.WhiteSmoke;
        public ShadingType ShadingType = ShadingType.Phong;

        public double Ks = 0.5;
        public double Kd = 0.5;
        public double Knvm = 0.5;
        public int M = 50;

        public bool DrawSpiralLight = true;

        public int Mr = 2;
        public int H = (int)CanvasSize.X/2;
    }
}
