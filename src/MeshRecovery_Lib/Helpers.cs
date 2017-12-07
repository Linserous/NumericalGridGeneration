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

        public static void Clear(ref int[][] numeration)
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

        public static bool NextPermutation<T>(T[] elements) where T : IComparable<T>
        {
            // More efficient to have a variable instead of accessing a property
            var count = elements.Length;

            // Indicates whether this is the last lexicographic permutation
            var done = false;

            // Go through the array from last to first
            for (var i = count - 1; i > 0; i--)
            {
                var curr = elements[i];

                // Check if the current element is less than the one before it
                if (curr.CompareTo(elements[i - 1]) < 0)
                {
                    continue;
                }

                // An element bigger than the one before it has been found,
                // so this isn't the last lexicographic permutation.
                done = true;

                // Save the previous (bigger) element in a variable for more efficiency.
                var prev = elements[i - 1];

                // Have a variable to hold the index of the element to swap
                // with the previous element (the to-swap element would be
                // the smallest element that comes after the previous element
                // and is bigger than the previous element), initializing it
                // as the current index of the current item (curr).
                var currIndex = i;

                // Go through the array from the element after the current one to last
                for (var j = i + 1; j < count; j++)
                {
                    // Save into variable for more efficiency
                    var tmp = elements[j];

                    // Check if tmp suits the "next swap" conditions:
                    // Smallest, but bigger than the "prev" element
                    if (tmp.CompareTo(curr) < 0 && tmp.CompareTo(prev) > 0)
                    {
                        curr = tmp;
                        currIndex = j;
                    }
                }

                // Swap the "prev" with the new "curr" (the swap-with element)
                elements[currIndex] = prev;
                elements[i - 1] = curr;

                // Reverse the order of the tail, in order to reset it's lexicographic order
                for (var j = count - 1; j > i; j--, i++)
                {
                    var tmp = elements[j];
                    elements[j] = elements[i];
                    elements[i] = tmp;
                }

                // Break since we have got the next permutation
                // The reason to have all the logic inside the loop is
                // to prevent the need of an extra variable indicating "i" when
                // the next needed swap is found (moving "i" outside the loop is a
                // bad practice, and isn't very readable, so I preferred not doing
                // that as well).
                break;
            }

            // Return whether this has been the last lexicographic permutation.
            return done;
        }
    }
}
