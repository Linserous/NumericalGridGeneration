using System;
using System.Collections.Generic;
using System.Linq;

namespace MeshRecovery_Lib
{
    public static partial class MeshRecovery
    {
        public class RecursiveAlgorithm
        {
            const int INVALID_INDEX = -1;

            enum ERROR
            {
                OK = 0,
                INVALID_DIM = 1,
                IMPOSSIBLE_NUM = 2
            }

            private Graph graph = null;
            private int[][] graphNumeration = null;

            public int TwoDimNumerate(Graph graph, out int[][] graphNumeration)
            {
                this.graph = graph;
                int verticesCount = graph.GetVerticesCount();
                this.graphNumeration = new int[verticesCount][];
                graphNumeration = this.graphNumeration;

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

                ERROR error = ERROR.OK;
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

                        // Step 3. Try to numerate neighbors vertices of the each vertex in first quad
                        foreach (var v in vertices)
                        {
                            error = NumerateNeighborVertices(v);
                            if (error != ERROR.OK)
                            {
                                NumerationHelpers.Clear(ref graphNumeration);
                                Helpers.Swap(ref vertices[0], ref vertices[vertices.Count() - times]);
                                execute = true;
                                times--;
                                break;
                            }
                        }
                        if (error != ERROR.OK) continue;

                        // Step 4. Try to numerate other ambiguous vertices
                        for (int i = 0; i < this.graphNumeration.Count(); ++i)
                        {
                            if (this.graphNumeration[i] == null)
                            {
                                int[] directions = new int[] { 0, 0, 0, 0 };
                                ERROR numerateError = ERROR.IMPOSSIBLE_NUM;
                                while (numerateError != ERROR.OK)
                                {
                                    // directions array has the following values:
                                    // [-x, -y, x, y]
                                    error = TryToNumerateVertex(i, ref directions);
                                    if (error != ERROR.OK)
                                    {
                                        NumerationHelpers.Clear(ref graphNumeration);
                                        Helpers.Swap(ref vertices[0], ref vertices[vertices.Count() - times]);
                                        execute = true;
                                        times--;
                                        break;
                                    }
                                    //TO DO: clear vertices if fail
                                    numerateError = NumerateNeighborVertices(i);
                                }
                            }
                            if (error != ERROR.OK) break;
                        }
                    }
                    return (int)error;
                }
                return (int)ERROR.INVALID_DIM;
            }

            private int[] NumerateFirstQuad(int rootVertex, int[] vertices)
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
                return vertices;
            }

            private ERROR NumerateNeighborVertices(int vertex)
            {
                ERROR error = ERROR.OK;
                int[] enumVertices;
                graph.GetAdjVertices(vertex, out enumVertices);
                foreach (var v in enumVertices)
                {
                    error = NumerateVertex(v);
                    if (error != ERROR.OK)
                    {
                        return error;
                    }
                }
                return error;
            }

            // Unambiguously numerate the vertex
            private ERROR NumerateVertex(int vertex)
            {
                if (graphNumeration[vertex] != null) return ERROR.OK;

                List<int> numVertices = new List<int>();
                int[] vertices;
                var verticesCount = graph.GetAdjVertices(vertex, out vertices);

                foreach (var v in vertices)
                {
                    if (graphNumeration[v] != null)
                    {
                        numVertices.Add(v);
                    }
                    else
                    {
                        // Checking whether all neighbors are numbered or not at the current vertex
                        List<int> currVertexNeighbors;
                        if (AllVerticesNumerated(v, out currVertexNeighbors))
                        {
                            if (!CalcVertexIndex(out graphNumeration[v], currVertexNeighbors))
                            {
                                return ERROR.INVALID_DIM;
                            }
                        }
                    }
                }

                // Checking if two or more neighbors are numbered at the current vertex
                if (numVertices.Count() >= 2)
                {
                    if (!CalcVertexIndex(out graphNumeration[vertex], numVertices))
                    {
                        return ERROR.INVALID_DIM;
                    }
                }
                return ERROR.OK;
            }

            // Try to number the vertex, which can not have an unambiguous index
            private ERROR TryToNumerateVertex(int vertex, ref int[] directions)
            {
                if (directions[3] != 0) return ERROR.IMPOSSIBLE_NUM;

                int[] index = null;
                int[] vertices;
                graph.GetAdjVertices(vertex, out vertices);
                bool newIndexFound = false;

                foreach (var v in vertices)
                {
                    if (graphNumeration[v] != null)
                    {
                        int x = 0, y = 0, i = 0;
                        while (!newIndexFound && directions[3] == 0)
                        {
                            while (directions[i] != 0) ++i;
                            directions[i] = (int)Math.Pow(-1, i / 2);
                            if (i % 2 == 0)
                            {
                                x = directions[i];
                            }
                            else
                            {
                                y = directions[i];
                            }

                            index = new int[] { graphNumeration[v][0] + x, graphNumeration[v][1] + y };

                            newIndexFound = !NumerationHelpers.IndexExists(index, ref graphNumeration);
                            if (newIndexFound) graphNumeration[vertex] = index;
                        }
                        break;
                    }
                }
                return newIndexFound ? ERROR.OK : ERROR.IMPOSSIBLE_NUM;
            }

            private bool CalcVertexIndex(out int[] index, List<int> vertices)
            {
                index = null;
                // Create 2 alternatives, for example:  [1, -1], [1, -2] from: [0, -1], [1, -2] 
                var alternative1 = new int[] { graphNumeration[vertices[0]][0], graphNumeration[vertices[1]][1] };
                var alternative2 = new int[] { graphNumeration[vertices[1]][0], graphNumeration[vertices[0]][1] };

                var alt1Exists = NumerationHelpers.IndexExists(alternative1, ref graphNumeration);
                var alt2Exists = NumerationHelpers.IndexExists(alternative2, ref graphNumeration);

                if (alt1Exists && alt2Exists) return false;

                int alt1Count = 0, alt2Count = 0;
                foreach (var v in vertices)
                {
                    if (!alt1Exists && CompareVertex(graphNumeration[v], alternative1, 2) >= 0) ++alt1Count;
                    if (!alt2Exists && CompareVertex(graphNumeration[v], alternative2, 2) >= 0) ++alt2Count;
                }
                if (alt1Count != vertices.Count() && alt2Count != vertices.Count()) return false;
                index = alt1Count == vertices.Count() ? alternative1 : alternative2;
                return true;
            }

            private bool AllVerticesNumerated(int vertex, out List<int> numVertices)
            {
                numVertices = new List<int>();
                int[] vertices;
                var verticesCount = graph.GetAdjVertices(vertex, out vertices);
                foreach (var v in vertices)
                {
                    if (graphNumeration[v] != null)
                    {
                        numVertices.Add(v);
                    }
                }
                return vertices.Count() == numVertices.Count();
            }
        }
    }
}
