using System;
using System.Collections.Generic;
using System.Linq;


namespace MeshRecovery_Lib
{
    public static partial class MeshRecovery
    {
        public partial class TwoDimNumerator
        {
            class VertexNumerator
            {
                enum Direction
                {
                    PositiveX = 0,
                    PositiveY,
                    NegativeX,
                    NegativeY,
                    Last
                }

                int vertex = INVALID_INDEX;
                Direction direction;
                List<int[]> alternatives = null;
                Graph graph = null;

                public VertexNumerator(int vertex, Graph graph)
                {
                    this.vertex = vertex;
                    alternatives = new List<int[]>();
                    this.graph = graph;
                }

                // Unambiguously numerate the vertex
                public Error Numerate(ref int[][] graphNumeration)
                {
                    if (graphNumeration[vertex] != null) return Error.OK;

                    int[] vertices;
                    graph.GetAdjVertices(vertex, out vertices);

                    // Checking whether 2 or more adj vertices are numbered or not at the current vertex
                    var adjVertices = GetNumeratedAdjVertices(graphNumeration);
                    if (adjVertices.Count() > 1)
                    {
                        if (!CalcVertexIndex(vertex, adjVertices, ref graphNumeration))
                        {
                            return Error.IMPOSSIBLE_NUM;
                        }
                    }
                    return Error.OK;
                }

                // Try to numerate the vertex, which can not have an unambiguous index
                public Error TryToNumerate(ref int[][] graphNumeration)
                {
                    if (direction == Direction.Last) return Error.IMPOSSIBLE_NUM;

                    var vertices = GetNumeratedAdjVertices(graphNumeration);
                    if (vertices.Count == 0)
                    {
                        return Error.NEED_MORE_DATA;
                    }
                    if (vertices.Count() > 1)
                    {
                        if (CalcVertexIndex(vertex, vertices, ref graphNumeration))
                        {
                            if (ContainsAlternative(graphNumeration[vertex]))
                            {
                                return Error.IMPOSSIBLE_NUM;
                            }
                            else
                            {
                                alternatives.Add(graphNumeration[vertex]);
                            }
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
                            newIndexFound = !NumerationHelpers.IndexExists(index, graphNumeration);
                            if (newIndexFound)
                            {
                                graphNumeration[vertex] = index;
                                if (!ContainsAlternative(index)) alternatives.Add(index);
                            }
                        }
                    }
                    return newIndexFound ? Error.OK : Error.IMPOSSIBLE_NUM;
                }

                public void Clear()
                {
                    alternatives.Clear();
                    direction = Direction.PositiveX;
                }

                public List<int> GetNumeratedAdjVertices(int[][] graphNumeration)
                {
                    return GetAdjVertices(graphNumeration, vertex, e => { return e != null; });
                }

                public List<int> GetEnumeratedAdjVertices(int[][] graphNumeration)
                {
                    var vertices = GetAdjVertices(graphNumeration, vertex, e => { return e == null; });
                    vertices.Sort((a, b) =>
                    {
                        var aAdjNumvertices = GetAdjVertices(graphNumeration, a, e => { return e != null; });
                        var bAdjNumvertices = GetAdjVertices(graphNumeration, b, e => { return e != null; });

                        return bAdjNumvertices.Count().CompareTo(aAdjNumvertices.Count());
                    });
                    return vertices;
                }

                delegate bool Callback(int[] index);
                private List<int> GetAdjVertices(int[][] graphNumeration, int vertex, Callback callback)
                {
                    List<int> result = new List<int>();
                    int[] vertices;
                    var verticesCount = graph.GetAdjVertices(vertex, out vertices);
                    foreach (var v in vertices)
                    {
                        if (callback(graphNumeration[v]))
                        {
                            result.Add(v);
                        }
                    }
                    return result;
                }

                private bool CalcVertexIndex(int vertex, List<int> vertices, ref int[][] graphNumeration)
                {
                    // Create 2 alternatives, for example:  [1, -1], [1, -2] from: [0, -1], [1, -2] 
                    var alternative1 = new int[] { graphNumeration[vertices[0]][0], graphNumeration[vertices[1]][1] };
                    var alternative2 = new int[] { graphNumeration[vertices[1]][0], graphNumeration[vertices[0]][1] };

                    var alt1Exists = NumerationHelpers.IndexExists(alternative1, graphNumeration);
                    var alt2Exists = NumerationHelpers.IndexExists(alternative2, graphNumeration);

                    if (alt1Exists && alt2Exists) return false;

                    int alt1Count = 0, alt2Count = 0;
                    foreach (var v in vertices)
                    {
                        if (!alt1Exists && CompareVertex(graphNumeration[v], alternative1, 2) >= 0) ++alt1Count;
                        if (!alt2Exists && CompareVertex(graphNumeration[v], alternative2, 2) >= 0) ++alt2Count;
                    }
                    if (alt1Count != vertices.Count() && alt2Count != vertices.Count()) return false;
                    graphNumeration[vertex] = alt1Count == vertices.Count() ? alternative1 : alternative2;
                    return true;
                }

                private bool ContainsAlternative(int[] alternative)
                {
                    foreach (var alt in alternatives)
                    {
                        if (alt[0] == alternative[0] && alt[1] == alternative[1]) return true;
                    }
                    return false;
                }
            }
        }
    }
}
