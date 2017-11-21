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

            enum Error
            {
                OK = 0,
                INVALID_DIM,
                IMPOSSIBLE_NUM,
                NEED_MORE_DATA
            }

            enum Direction
            {
                PositiveX = 0,
                PositiveY,
                NegativeX,
                NegativeY,
                Last
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

                        // Step 3. Try to numerate neighbors vertices of the each vertex in first quad
                        foreach (var v in vertices)
                        {
                            List<int> numerated = new List<int>();
                            error = NumerateNeighborVertices(v, ref numerated);
                            if (error != Error.OK) break;
                        }

                        if (error == Error.OK)
                        {
                            // Step 4. Try to numerate other ambiguous vertices
                            error = TryToNumerateOtherVertices();
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

            private Error NumerateNeighborVertices(int vertex, ref List<int> numerated)
            {
                var error = Error.OK;
                int[] enumVertices;
                graph.GetAdjVertices(vertex, out enumVertices);
                foreach (var v in enumVertices)
                {
                    if (graphNumeration[v] != null) continue;
                    error = NumerateVertex(v);
                    if (error != Error.OK)
                    {
                        return error;
                    }
                    numerated.Add(v);
                }
                return error;
            }

            // Unambiguously numerate the vertex
            private Error NumerateVertex(int vertex)
            {
                if (graphNumeration[vertex] != null) return Error.OK;

                int[] vertices;
                var verticesCount = graph.GetAdjVertices(vertex, out vertices);
                // Checking whether 2 or more neighbors are numbered or not at the current vertex
                var currVertexNeighbors = GetNumeratedVertices(vertex);
                if (currVertexNeighbors.Count() > 1)
                {
                    if (!CalcVertexIndex(out graphNumeration[vertex], currVertexNeighbors))
                    {
                        return Error.IMPOSSIBLE_NUM;
                    }
                }
                return Error.OK;
            }

            // Try to numerate the vertex, which can not have an unambiguous index
            private Error TryToNumerateVertex(int vertex, ref Direction direction, ref List<int[]> usedAlternatives)
            {
                if (direction == Direction.Last) return Error.IMPOSSIBLE_NUM;

                var vertices = GetNumeratedVertices(vertex);
                if(vertices.Count == 0)
                {
                    return Error.NEED_MORE_DATA;
                }
                if (vertices.Count() > 1)
                {
                    if (CalcVertexIndex(out graphNumeration[vertex], vertices))
                    {
                        //TO DO: fix WA
                        if (usedAlternatives.Count() > 0)
                        {
                            return Error.IMPOSSIBLE_NUM;
                        }
                        usedAlternatives.Add(graphNumeration[vertex]);
                        return Error.OK;
                    }
                    return Error.IMPOSSIBLE_NUM;
                }
                int[] index = null;
                bool newIndexFound = false;
                if (graphNumeration[vertices[0]] != null)
                {
                    while (!newIndexFound && direction != Direction.Last)
                    {
                        var directionValue = (int)Math.Pow(-1, (int)direction / 2);
                        int x = (int)direction % 2 == 0 ? directionValue : 0;
                        int y = (int)direction % 2 != 0 ? directionValue : 0;
                        ++direction;

                        index = new int[] { graphNumeration[vertices[0]][0] + x, graphNumeration[vertices[0]][1] + y };
                        newIndexFound = !NumerationHelpers.IndexExists(index, ref graphNumeration);
                        if (newIndexFound) graphNumeration[vertex] = index;
                    }
                }
                return newIndexFound ? Error.OK : Error.IMPOSSIBLE_NUM;
            }

            private Error TryToNumerateOtherVertices()
            {
                var error = Error.OK;
                var enumerated = GetEnumeratedVertices();
                var numerated = new List<int>[enumerated.Count()];
                var directions = new Direction[enumerated.Count()];
                var usedAliternatives = new List<int[]>[enumerated.Count];
                int i = 0;
                while (i < enumerated.Count() && i > -1)
                {
                    if (numerated[i] != null)
                    {
                        if (error != Error.OK)
                        {
                            NumerationHelpers.Clear(ref graphNumeration, numerated[i]);
                            numerated[i].Clear();
                            graphNumeration[enumerated[i]] = null;
                        }
                    }
                    else
                    {
                        numerated[i] = new List<int>();
                        usedAliternatives[i] = new List<int[]>();
                    }
                    if (graphNumeration[enumerated[i]] == null)
                    {
                        var numerateError = Error.IMPOSSIBLE_NUM;
                        while (numerateError != Error.OK)
                        {
                            if (numerated[i].Count() != 0)
                            {
                                NumerationHelpers.Clear(ref graphNumeration, numerated[i]);
                                numerated[i].Clear();
                                graphNumeration[enumerated[i]] = null;
                            }

                            error = TryToNumerateVertex(enumerated[i], ref directions[i], ref usedAliternatives[i]);
                            if (error != Error.OK) break;

                            numerated[i].Add(enumerated[i]);
                            if (GetNumeratedVertices(enumerated[i]).Count() == 0)
                            {
                                numerateError = Error.OK;
                            }
                            else
                            {
                                numerateError = NumerateNeighborVertices(enumerated[i], ref numerated[i]);
                            }
                        }
                        if (error != Error.OK) directions[i] = Direction.PositiveX;
                        i += (error == Error.OK || error == Error.NEED_MORE_DATA) ? 1 : -1;
                    }
                    else
                    {
                        ++i;
                    }
                }
                return error;
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

            private List<int> GetNumeratedVertices(int vertex)
            {
                List<int> numVertices = new List<int>();
                int[] vertices;
                var verticesCount = graph.GetAdjVertices(vertex, out vertices);
                foreach (var v in vertices)
                {
                    if (graphNumeration[v] != null)
                    {
                        numVertices.Add(v);
                    }
                }
                return numVertices;
            }

            private List<int> GetEnumeratedVertices()
            {
                List<int> enumerated = new List<int>();
                for (int i = 0; i < graphNumeration.Count(); ++i)
                {
                    if (graphNumeration[i] == null)
                    {
                        if (GetNumeratedVertices(i).Count() > 0)
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
