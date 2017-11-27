using System;
using System.Collections.Generic;
using System.Linq;

namespace MeshRecovery_Lib
{
    public static partial class MeshRecovery
    {
        // recursive version of the two-dimensional numbering algorithm
        public class ThreeDimNumerator : ANumerator<ThreeDimVertexNumerator>
        {
            protected override int GetMaxVertexDegree()
            {
                return 6;
            }
            protected override void NumerateFirstVertices(int rootVertex, int[] vertices)
            {
                graphNumeration[rootVertex] = new int[] { 0, 0, 0 };
                int x = 0, y = 0, z = -1;
                for (int i = 0; i < vertices.Count(); ++i)
                {
                    graphNumeration[vertices[i]] = new int[] { x, y, z };
                    Helpers.Swap3(ref x, ref y, ref z);
                    if (i > 1)
                    {
                        x *= i > 0 ? x : 1;
                        y *= i > 0 ? y : 1;
                        z *= i > 0 ? z : 1;
                    }
                }
            }
        }
    }
}
