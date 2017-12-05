﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MeshRecovery_Lib
{
    public interface INumerator
    {
        int Run(Graph graph, out int[][] graphNumeration);
    }

    public abstract class ANumerator<T> : INumerator where T : IVertexNumerator, new()
    {
        public const int INVALID_INDEX = -1;

        protected Graph graph = null;
        protected int[][] graphNumeration = null;
        protected T[] numerators = null;

        protected class VertexData
        {
            public int Parent { get; set; }
            public List<int> Children { get; set; }

            public VertexData()
            {
                Parent = INVALID_INDEX;
                Children = new List<int>();
            }

            public void Clear()
            {
                Parent = INVALID_INDEX;
                Children.Clear();
            }
        }
        protected VertexData[] verticesData = null;


        // abstract members
        protected abstract void NumerateFirstVertices(int rootVertex, int[] vertices);
        protected abstract bool Swap(ref int[] vertices);

        private void InitMembers(Graph graph, out int[][] graphNumeration)
        {
            this.graph = graph;
            int vertexCount = graph.GetVerticesCount();
            this.graphNumeration = new int[vertexCount][];
            graphNumeration = this.graphNumeration;

            numerators = new T[vertexCount];
            verticesData = new VertexData[vertexCount];

            for (int i = 0; i < numerators.Count(); ++i)
            {
                (numerators[i] = new T()).Init(i, graph);
                verticesData[i] = new VertexData();
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

                bool possibleToSwap = true;
                while (possibleToSwap)
                {
                    NumerationHelper.Clear(ref graphNumeration);
                    foreach (var numerator in numerators) numerator.Clear();
                    foreach (var data in verticesData) data.Clear();
                    possibleToSwap = false;

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
                        for (int i = 0; i < enumerated.Count(); ++i)
                        {
                            error = TryToNumerateVertices(enumerated[i]);
                            if (error != Error.OK) break;
                        }
                    }
                    possibleToSwap = error != Error.OK && Swap(ref vertices);

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
            if (graphNumeration[vertex] != null) return Error.OK;
            var error = Error.OK;
            var list = new List<int>();
            list.Add(vertex);

            while (list.Count() != 0)
            {
                var v = list.Last();
                list.RemoveAt(list.Count() - 1);
                if (graphNumeration[v] != null) continue;
                error = numerators[v].TryToNumerate(ref graphNumeration);

                if (error != Error.OK)
                {
                    var parent = verticesData[v].Parent;
                    if (parent == INVALID_INDEX) return Error.IMPOSSIBLE_NUM;

                    list.Add(parent);
                    ClearVertexChildren(parent);
                    graphNumeration[parent] = null;
                    continue;
                }

                var vertices = numerators[v].GetEnumeratedAdjVertices(graphNumeration);
                verticesData[v].Children = vertices;

                for (int i = vertices.Count() - 1; i > -1; --i)
                {
                    if (list.Contains(vertices[i]))
                    {
                        list.Remove(vertices[i]);
                    }
                    list.Add(vertices[i]);
                    verticesData[vertices[i]].Parent = v;
                }
            }
            return error;
        }

        private void ClearVertex(int vertex)
        {
            numerators[vertex].Clear();
            verticesData[vertex].Parent = INVALID_INDEX;
            graphNumeration[vertex] = null;
        }

        private void ClearVertexChildren(int vertex)
        {
            foreach (var child in verticesData[vertex].Children)
            {
                ClearVertexChildren(child);
                ClearVertex(child);
            }
            verticesData[vertex].Children.Clear();
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
