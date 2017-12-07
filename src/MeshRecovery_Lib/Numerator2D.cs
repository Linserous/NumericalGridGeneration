using System.Linq;

namespace MeshRecovery_Lib
{
    public static partial class MeshRecovery
    {
        // recursive version of the two-dimensional numbering algorithm
        public class Numerator2D : ANumerator<VertexNumerator2D>
        {
            protected override void NumerateFirstVertices(int rootVertex, int[] vertices)
            {
                graphNumeration[rootVertex] = new int[] { 0, 0 };
                int x = 0, y = -1;
                for (int i = 0; i < vertices.Count(); ++i)
                {
                    graphNumeration[vertices[i]] = new int[] { x, y };
                    Helpers.Swap(ref x, ref y);
                    x *= i > 0 ? x : 1;
                    y *= i > 0 ? y : 1;
                }
            }

            protected override bool Swap(ref int[] vertices)
            {
                return !Helpers.NextPermutation(vertices);
            }
        }
    }
}
