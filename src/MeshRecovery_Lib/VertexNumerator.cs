using System.Collections.Generic;
using System.Linq;

namespace MeshRecovery_Lib
{
    public class VertexNumerator<T> : IVertexNumerator where T : IDirection, new()
    {
        const int INVALID_VERTEX = -1;

        int vertex = INVALID_VERTEX;
        T direction;
        List<int[]> alternatives = null;
        Graph graph = null;

        public virtual void Init(int vertex, Graph graph)
        {
            direction = new T();
            this.vertex = vertex;
            alternatives = new List<int[]>();
            this.graph = graph;
        }

        public virtual Error Numerate(int[][] graphNumeration)
        {
            if (graphNumeration[vertex] != null) return Error.OK;

            // Checking whether 2 or more adj vertices are numbered or not at the current vertex
            var adjVertices = GetNumeratedAdjVertices(graphNumeration);
            if (adjVertices.Count() > 1)
            {
                var alts = CalcVertexIndex(vertex, adjVertices, graphNumeration);
                if (alts == null)
                {
                    return Error.IMPOSSIBLE_NUM;
                }
                graphNumeration[vertex] = alts.First();
            }
            return Error.OK;
        }

        public virtual Error TryToNumerate(int[][] graphNumeration)
        {
            if (!direction.Valid()) return Error.IMPOSSIBLE_NUM;

            var vertices = GetNumeratedAdjVertices(graphNumeration);
            if (vertices.Count == 0)
            {
                return Error.NEED_MORE_DATA;
            }
            if (vertices.Count() > 1)
            {
                var alts = CalcVertexIndex(vertex, vertices, graphNumeration);
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
                while (!newIndexFound && direction.Valid())
                {
                    var offset = direction.GetNextOffset();
                    index = new int[offset.Count()];
                    for (int i = 0; i < offset.Count(); ++i)
                    {
                        index[i] = graphNumeration[vertices[0]][i] + offset[i];
                    }
                    newIndexFound = !NumerationHelper.IndexExists(index, graphNumeration);
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
            direction.Clear();
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

                var result = aNumerated.Count().CompareTo(bNumerated.Count());
                if (result == 0)
                {
                    result = aAdjVertices.Count().CompareTo(bAdjVertices.Count());
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

        private List<int[]> CalcVertexIndex(int vertex, List<int> vertices, int[][] graphNumeration)
        {
            List<int[]> alts = new List<int[]>();
            var localDirection = new T();
            int v_0 = vertices[0];
            while (localDirection.Valid())
            {
                var offset = localDirection.GetNextOffset();
                var alternative = new int[offset.Count()];
                for (int i = 0; i < alternative.Count(); ++i)
                {
                    alternative[i] = graphNumeration[v_0][i] + offset[i];
                }
                int altCount = 0;
                if (!NumerationHelper.IndexExists(alternative, graphNumeration))
                {
                    foreach (var v in vertices)
                    {
                        if (NumerationHelper.CompareVertex(graphNumeration[v], alternative, alternative.Count()) >= 0) ++altCount;
                    }
                    if (altCount == vertices.Count()) alts.Add(alternative);
                }
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
    }

    public class VertexNumerator2D : VertexNumerator<Direction2D>
    {
    }
    public class VertexNumerator3D : VertexNumerator<Direction3D>
    {
    }
}