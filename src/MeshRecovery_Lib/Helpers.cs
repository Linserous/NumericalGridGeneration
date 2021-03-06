﻿using System.Linq;
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
            if (v1.Length != v2.Length || v1.Length < meshDimension) return -2;

            int diff_count = 0;
            int diff_index = -1;
            for (int i = 0; i < v1.Length; ++i)
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

        public static void Clear(int[][] numeration)
        {
            for (int i = 0; i < numeration.Count(); ++i)
            {
                numeration[i] = null;
            }
        }

        public static bool IndexExists(int[] index, int[][] numeration)
        {
            for (int i = 0; i < numeration.Count(); ++i)
            {
                if (numeration[i] != null)
                {
                    var numElementCount = 0;
                    for (int j = 0; j < index.Count(); ++j)
                    {
                        if (numeration[i][j] == index[j]) ++numElementCount;
                    }
                    if (numElementCount == index.Count()) return true;
                }
            }
            return false;
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

        public static void Swap3<T>(ref T a, ref T b, ref T c)
        {
            T temp = a;
            a = b;
            b = c;
            c = temp;
        }

        public static bool GetNextPermutation<T>(T[] numList) where T : IComparable<T>
        {
            /*
             Knuths
             1. Find the largest index j such that a[j] < a[j + 1]. If no such index exists, the permutation is the last permutation.
             2. Find the largest index l such that a[j] < a[l]. Since j + 1 is such an index, l is well defined and satisfies j < l.
             3. Swap a[j] with a[l].
             4. Reverse the sequence from a[j + 1] up to and including the final element a[n].

             */
            var largestIndex = -1;
            for (var i = numList.Length - 2; i >= 0; i--)
            {
                if (numList[i].CompareTo(numList[i + 1]) < 0)
                {
                    largestIndex = i;
                    break;
                }
            }

            if (largestIndex < 0) return false;

            var largestIndex2 = -1;
            for (var i = numList.Length - 1; i >= 0; i--)
            {
                if (numList[largestIndex].CompareTo(numList[i]) < 0)
                {
                    largestIndex2 = i;
                    break;
                }
            }

            Swap(ref numList[largestIndex], ref numList[largestIndex2]);

            for (int i = largestIndex + 1, j = numList.Length - 1; i < j; i++, j--)
            {
                Swap(ref numList[i], ref numList[j]);
            }

            return true;
        }
    }
}
