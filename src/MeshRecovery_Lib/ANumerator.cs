using System;
using System.Collections.Generic;
using System.Linq;

namespace MeshRecovery_Lib
{
    public abstract class ANumerator<T> where T : IVertexNumerator, new()
    {
        public const int INVALID_INDEX = -1;

        protected Graph graph = null;
        protected int[][] graphNumeration = null;
        protected IVertexNumerator[] numerators = null;
        int TryToNumStackCounter = 0;
        const int stackSize = 2000;

        // abstract members
        protected abstract int GetMaxVertexDegree();
        protected abstract void NumerateFirstVertices(int rootVertex, int[] vertices);
        protected abstract bool Swap(ref int[] vertices);

        private void InitMembers(Graph graph, out int[][] graphNumeration)
        {
            this.graph = graph;
            int vertexCount = graph.GetVerticesCount();
            this.graphNumeration = new int[vertexCount][];
            graphNumeration = this.graphNumeration;

            numerators = new IVertexNumerator[vertexCount];
            for (int i = 0; i < numerators.Count(); ++i)
            {
                (numerators[i] = new T()).Init(i, graph);
            }
        }

        public int Run(Graph graph, out int[][] graphNumeration)
        {
            InitMembers(graph, out graphNumeration);

            int vertexWithMaxDegree = INVALID_INDEX;
            long maxDegree = 0;

            var traversal = new Traversal<BFS>(graph);
            //Step 1. Find start vertex with max degree
            traversal.NewVertex = (sender, e) =>
            {
                long currDegree = graph.GetAdjVerticesCount(e);
                if (currDegree > maxDegree)
                {
                    vertexWithMaxDegree = e;
                    maxDegree = currDegree;
                }
            };
            traversal.Run();

            Error error = Error.OK;
            if (vertexWithMaxDegree != INVALID_INDEX)
            {
                int[] vertices;
                graph.GetAdjVertices(vertexWithMaxDegree, out vertices);

                bool execute = true;
                while (execute)
                {
                    execute = false;

                    // Step 2. Numerate first vertices
                    NumerateFirstVertices(vertexWithMaxDegree, vertices);

                    // Step 3. Try to numerate adj vertices of the each vertex in the first vertices
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
                            if (error == Error.STACKOVERFLOW) return (int)error;
                            if (error != Error.OK && i == 0) break;
                            i += error != Error.OK ? -1 : 1;
                        }
                    }
                    if (error != Error.OK)
                    {
                        execute = Swap(ref vertices);
                        if (execute) NumerationHelper.Clear(ref graphNumeration);
                    }
                }
                return (int)error;
            }
            return (int)Error.INVALID_DIM;
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
                if (error != Error.OK) return error;
            }
            return error;
        }

        private Error TryToNumerateVertices(int vertex)
        {
            //Workaround to prevent StackOverflow exception
            if (TryToNumStackCounter >= stackSize) return Error.STACKOVERFLOW;
            if (graphNumeration[vertex] != null) return Error.OK;
            ++TryToNumStackCounter;

            Error error = Error.IMPOSSIBLE_NUM;
            while (error != Error.OK && error != Error.NEED_MORE_DATA)
            {
                graphNumeration[vertex] = null;
                error = numerators[vertex].TryToNumerate(ref graphNumeration);

                if (error != Error.OK)
                {
                    ClearVertex(vertex);
                    --TryToNumStackCounter;
                    return error;
                }

                var vertices = numerators[vertex].GetEnumeratedAdjVertices(graphNumeration);
                for (int i = 0; i < vertices.Count(); ++i)
                {
                    error = TryToNumerateVertices(vertices[i]);
                    if (error == Error.STACKOVERFLOW)
                    {
                        --TryToNumStackCounter;
                        return error;
                    }
                    if (error != Error.OK)
                    {
                        while (i-- != 0) ClearVertex(i);
                        break;
                    }
                }
            }
            --TryToNumStackCounter;
            return error;
        }

        private void ClearVertex(int vertex)
        {
            numerators[vertex].Clear();
            graphNumeration[vertex] = null;
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
                        enumerated.Add(i);
                    }
                }
            }
            enumerated.Sort((a, b) =>
            {
                var aAdjNumvertexCount = graph.GetAdjVerticesCount(a);
                var bAdjNumvertexCount = graph.GetAdjVerticesCount(b);
                return bAdjNumvertexCount.CompareTo(aAdjNumvertexCount);
            });
            return enumerated;
        }
    }
}
