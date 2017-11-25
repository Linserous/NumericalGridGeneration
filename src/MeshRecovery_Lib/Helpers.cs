using System.Linq;
using System.Collections.Generic;
using System;

namespace MeshRecovery_Lib
{
    public static class NumerationHelper
    {
        public static int ValidateNumeration(long[] xadj, int[] adjncy, int meshDimension, int[][] graphNumeration)
        {
            int result = 0;
            for (int k = 0; k < graphNumeration.Length - 1; ++k)
            {
                for (int l = k + 1; l < graphNumeration.Length; ++l)
                {
                    int res = CompareVertex(graphNumeration[l], graphNumeration[k], meshDimension);
                    if (res == -1)
                        return res;
                }
            }
            Graph graph = new Graph(xadj, adjncy);
            Traversal traversal = new Traversal(graph);
            traversal.NewVertex += (sender, e) =>
            {
                int[] adj;
                long adj_count = graph.GetAdjVertices(e, out adj);
                for (int k = 0; k < adj_count; ++k)
                {
                    int res = CompareVertex(graphNumeration[e], graphNumeration[adj[k]], meshDimension);
                    if (res < 0 && result == 0)
                        result = res;
                }
            };
            traversal.Run();
            return result;
        }
        // CompareVertex - will find a difference btw vertex
        // Output:
        // i  - [0,..,meshDimension-1] index of difference
        // -1 - same numerate of indexes
        // -2 - length of vertex numerating is different
        // -3 - there are two or more difference
        // -4 - there is difference with step more than 1,  like [0,0] [0,2] 
        internal static int CompareVertex(int[] v1, int[] v2, int meshDimension)
        {
            if ((v1.Length != meshDimension) || (v2.Length != meshDimension)) return -2;
            int diff_count = 0;
            int diff_index = -1;
            for (int i = 0; i < meshDimension; ++i)
            {
                if (Math.Abs(v1[i] - v2[i]) == 1)
                {
                    diff_index = i;
                    ++diff_count;
                }
                if (Math.Abs(v1[i] - v2[i]) > 1)
                {
                    return -4;
                }
            }
            if (diff_count > 1) return -3;
            return diff_index;
        }
    }

    static class Helpers
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }

    static class NumerationHelpers
    {
        public static void Clear(ref int[][] numeration)
        {
            for (int i = 0; i < numeration.Count(); ++i)
            {
                numeration[i] = null;
            }
        }

        public static void Clear(ref int[][] numeration, List<int> vertices)
        {
            foreach (var v in vertices)
            {
                numeration[v] = null;
            }
        }

        public static bool IndexExists(int[] index, int[][] numeration)
        {
            for (int i = 0; i < numeration.Count(); ++i)
            {
                if (numeration[i] != null &&
                    numeration[i][0] == index[0] &&
                    numeration[i][1] == index[1]) return true;
            }
            return false;
        }
    }
}
