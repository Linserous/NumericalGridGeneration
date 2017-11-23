using System;
using System.Collections.Generic;
using System.Linq;

namespace MeshRecovery_Lib
{
    public static partial class MeshRecovery
    {
        // recursive version of the two-dimensional numbering algorithm
        public partial class TwoDimNumerator
        {
            const int INVALID_INDEX = -1;

            enum Error
            {
                OK = 0,
                INVALID_DIM,
                IMPOSSIBLE_NUM,
                NEED_MORE_DATA
            }

            private Graph graph = null;
            private int[][] graphNumeration = null;
            private VertexNumerator[] numerators = null;

            private void InitMembers(Graph graph, out int[][] graphNumeration)
            {
                this.graph = graph;
                int vertexCount = graph.GetVerticesCount();
                this.graphNumeration = new int[vertexCount][];
                graphNumeration = this.graphNumeration;

                numerators = new VertexNumerator[vertexCount];
                for (int i = 0; i < numerators.Count(); ++i)
                {
                    numerators[i] = new VertexNumerator(i, graph);
                }
            }

            public int Run(Graph graph, out int[][] graphNumeration)
            {
                InitMembers(graph, out graphNumeration);

                int vertex4 = INVALID_INDEX;
                int vertex3 = INVALID_INDEX;

                var traversal = new Traversal<BFS>(graph);
                //Step 1. Find start vertex with 4 or 3 degree
                traversal.NewVertex = (sender, e) =>
                {
                    if (graph.GetAdjVerticesCount(e) == 4)
                    {
                        vertex4 = e;
                        traversal.Stop();
                    }
                    else if (graph.GetAdjVerticesCount(e) == 3) vertex3 = e;
                };
                traversal.Run();

                Error error = Error.OK;
                if (vertex4 != INVALID_INDEX || vertex3 != INVALID_INDEX)
                {
                    var vertex = vertex4 != INVALID_INDEX ? vertex4 : vertex3;
                    int[] vertices;
                    graph.GetAdjVertices(vertex, out vertices);

                    bool execute = true;
                    int times = vertices.Count() - 1;
                    while (execute && times > 0)
                    {
                        execute = false;

                        // Step 2. Numerate first quad or first triplet
                        NumerateFirstQuad(vertex, vertices);

                        // Step 3. Try to numerate adj vertices of the each vertex in the first quad
                        foreach (var v in vertices)
                        {
                            error = NumerateAdjVertices(v);
                            if (error != Error.OK) break;
                        }

                        // Step 4. Try to numerate other ambiguous vertices
                        if (error == Error.OK)
                        {
                            var enumerated = GetEnumeratedVertices();
                            for (int i = 0; i < enumerated.Count();)
                            {
                                error = TryToNumerateVertices(enumerated[i]);
                                if (error != Error.OK && i == 0)
                                {
                                    break;
                                }
                                i += error != Error.OK ? -1 : 1;
                            }
                        }
                        if (error != Error.OK)
                        {
                            Helpers.Swap(ref vertices[0], ref vertices[vertices.Count() - times]);
                            execute = true;
                            if (--times > 0) NumerationHelpers.Clear(ref graphNumeration);
                        }
                    }
                    return (int)error;
                }
                return (int)Error.INVALID_DIM;
            }

            private void NumerateFirstQuad(int rootVertex, int[] vertices)
            {
                graphNumeration[rootVertex] = new int[] { 0, 0 };

                // Fill adj vertices with the following values:
                //-1 0, 0 -1, 1 0, 0 1
                int x = 0, y = -1;
                for (int i = 0; i < vertices.Count(); ++i)
                {
                    graphNumeration[vertices[i]] = new int[] { x, y };
                    Helpers.Swap(ref x, ref y);
                    x *= i > 0 ? x : 1;
                    y *= i > 0 ? y : 1;
                }
            }

            private Error NumerateAdjVertices(int vertex)
            {
                var error = Error.OK;
                int[] enumVertices;
                graph.GetAdjVertices(vertex, out enumVertices);
                foreach (var v in enumVertices)
                {
                    if (graphNumeration[v] != null) continue;
                    error = numerators[v].Numerate(ref graphNumeration);
                    if (error != Error.OK)
                    {
                        return error;
                    }
                }
                return error;
            }

            private Error TryToNumerateVertices(int vertex)
            {
                if (graphNumeration[vertex] != null) return Error.OK;

                Error error = Error.IMPOSSIBLE_NUM;

                int[] vertices;
                var vertexCount = graph.GetAdjVertices(vertex, out vertices);

                while (error != Error.OK)
                {
                    graphNumeration[vertex] = null;
                    error = numerators[vertex].TryToNumerate(ref graphNumeration);
                    if (error != Error.OK)
                    {
                        numerators[vertex].Clear();
                        graphNumeration[vertex] = null;
                        return error;
                    }
                    int prevI = 0;
                    for (int i = 0; i < vertexCount;)
                    {
                        if (graphNumeration[vertices[i]] != null)
                        {
                            var temp = i;
                            i += i < prevI ? -1 : 1;
                            prevI = temp;
                            if (i <= 0) break;
                            continue;
                        }
                        error = TryToNumerateVertices(vertices[i]);
                        if (error != Error.OK)
                        {
                            numerators[vertices[i]].Clear();
                            graphNumeration[vertices[i]] = null;
                            if (i == 0) break;
                        }
                        prevI = i;
                        i += (error == Error.OK || error == Error.NEED_MORE_DATA) ? 1 : -1;
                    }
                }
                return error;
            }

            private List<int> GetEnumeratedVertices()
            {
                List<int> enumerated = new List<int>();
                for (int i = 0; i < graphNumeration.Count(); ++i)
                {
                    if (graphNumeration[i] == null)
                    {
                        if (numerators[i].GetNumeratedAdjVertices(graphNumeration).Count() > 0)
                        {
                            enumerated.Insert(0, i);
                        }
                        else
                        {
                            enumerated.Add(i);
                        }
                    }
                }
                return enumerated;
            }
        }
    }
}
