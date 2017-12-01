using System;
using System.Collections.Generic;
using System.Linq;


namespace MeshRecovery_Lib
{
    public static partial class MeshRecovery
    {
        public class ThreeDimVertexNumerator : IVertexNumerator
        {
            enum Direction
            {
                PositiveX = 0,
                PositiveY,
                PositiveZ,
                NegativeX,
                NegativeY,
                NegativeZ,
                Last
            }

            int vertex = -1;
            Direction direction;
            List<int[]> alternatives = null;
            Graph graph = null;

            public virtual void Init(int vertex, Graph graph)
            {
                this.vertex = vertex;
                alternatives = new List<int[]>();
                this.graph = graph;
            }

            public virtual Error Numerate(ref int[][] graphNumeration)
            {
                if (graphNumeration[vertex] != null) return Error.OK;

                // Checking whether 2 or more adj vertices are numbered or not at the current vertex
                var adjVertices = GetNumeratedAdjVertices(graphNumeration);
                if (adjVertices.Count() > 1)
                {
                    var alts = CalcVertexIndex(vertex, adjVertices, ref graphNumeration);
                    if (alts == null)
                    {
                        return Error.IMPOSSIBLE_NUM;
                    }
                    // tmp
                    graphNumeration[vertex] = alts.First();
                }
                return Error.OK;
            }

            public virtual Error TryToNumerate(ref int[][] graphNumeration)
            {
                if (direction == Direction.Last) return Error.IMPOSSIBLE_NUM;

                var vertices = GetNumeratedAdjVertices(graphNumeration);
                if (vertices.Count == 0)
                {
                    return Error.NEED_MORE_DATA;
                }
                if (vertices.Count() > 1)
                {
                    var alts = CalcVertexIndex(vertex, vertices, ref graphNumeration);
                    if (alts != null)
                    {
                        foreach (var alternative in alts)
                        {
                            if (!ContainsAlternative(alternative))
                            {
                                graphNumeration[vertex] = alternative;
                                alternatives.Add(alternative);
                                return Error.OK;
                            }
                        }
                    }
                    return Error.IMPOSSIBLE_NUM;
                }
                int[] index = null;
                bool newIndexFound = false;
                if (graphNumeration[vertices[0]] != null)
                {
                    while (!newIndexFound && direction != Direction.Last)
                    {
                        var offset = GetOffsetByDirection(direction);
                        ++direction;

                        index = new int[] { graphNumeration[vertices[0]][0] + offset[0],
                            graphNumeration[vertices[0]][1] + offset[1],
                            graphNumeration[vertices[0]][2] + offset[2] };

                        newIndexFound = !NumerationHelper.IndexExists3(index, graphNumeration);
                        if (newIndexFound)
                        {
                            graphNumeration[vertex] = index;
                            if (!ContainsAlternative(index)) alternatives.Add(index);
                        }
                    }
                }
                return newIndexFound ? Error.OK : Error.IMPOSSIBLE_NUM;
            }

            public virtual void Clear()
            {
                alternatives.Clear();
                direction = Direction.PositiveX;
            }

            public virtual List<int> GetNumeratedAdjVertices(int[][] graphNumeration)
            {
                return GetAdjVertices(graphNumeration, vertex, e => { return e != null; });
            }

            public virtual List<int> GetEnumeratedAdjVertices(int[][] graphNumeration)
            {
                var vertices = GetAdjVertices(graphNumeration, vertex, e => { return e == null; });
                vertices.Sort((a, b) =>
                {
                    List<int[]> aNumerated = new List<int[]>();
                    List<int[]> bNumerated = new List<int[]>();
                    var aAdjVertices = GetAdjVertices(graphNumeration, a, e => { if (e != null) { aNumerated.Add(e); } return true; });
                    var bAdjVertices = GetAdjVertices(graphNumeration, b, e => { if (e != null) { bNumerated.Add(e); } return true; });

                    var result = bNumerated.Count().CompareTo(aNumerated.Count());
                    if (result == 0)
                    {
                        result = bAdjVertices.Count().CompareTo(aAdjVertices.Count());
                    }
                    return result;
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

            private List<int[]> CalcVertexIndex(int vertex, List<int> vertices, ref int[][] graphNumeration)
            {
                List<int[]> alts = new List<int[]>();
                var localDirection = Direction.PositiveX;
                int v_0 = vertices[0];
                while (localDirection != Direction.Last)
                {
                    var offset = GetOffsetByDirection(localDirection);
                    var alternative = new int[] { graphNumeration[v_0][0] + offset[0],
                        graphNumeration[v_0][1] + offset[1],
                        graphNumeration[v_0][2] + offset[2] };
                    int altCount = 0;
                    if (!NumerationHelper.IndexExists3(alternative, graphNumeration))
                    {
                        foreach (var v in vertices)
                        {
                            if (NumerationHelper.CompareVertex(graphNumeration[v], alternative, 3) >= 0) ++altCount;
                        }
                        if (altCount == vertices.Count()) alts.Add(alternative);
                    }
                    ++localDirection;
                }
                return alts.Count() == 0 ? null : alts;
            }

            private bool ContainsAlternative(int[] alternative)
            {
                foreach (var alt in alternatives)
                {
                    int numberOfCoincidences = 0;
                    var altCount = alt.Count();
                    for (int i = 0; i < altCount; ++i)
                    {
                        if (alt[i] == alternative[i]) ++numberOfCoincidences;
                    }
                    if (numberOfCoincidences == altCount) return true;
                }
                return false;
            }

            private int[] GetOffsetByDirection(Direction d)
            {
                var directionValue = (int)Math.Pow(-1, (int)d / 3);
                int x = (int)d % 3 == 0 ? directionValue : 0;
                int y = (int)d % 3 == 1 ? directionValue : 0;
                int z = (int)d % 3 == 2 ? directionValue : 0;
                return new int[] { x, y, z };
            }
        }
    }
}
