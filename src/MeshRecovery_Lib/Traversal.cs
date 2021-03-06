﻿using System;
using System.Linq;

namespace MeshRecovery_Lib
{
    using TraversalHelpers;

    public interface IFS
    {
        void Init(Graph graph);
        IContainer GetContainer();
    }

    public class DFS : IFS
    {
        private DFSContainer container;

        public void Init(Graph graph)
        {
            container = new DFSContainer(graph.GetVerticesCount());
        }

        public IContainer GetContainer()
        {
            return container;
        }
    }

    public class BFS : IFS
    {
        private BFSContainer container;

        public void Init(Graph graph)
        {
            container = new BFSContainer(graph.GetVerticesCount());
        }

        public IContainer GetContainer()
        {
            return container;
        }
    }
    public class Traversal<T> where T : IFS, new()
    {
        private Graph graph;
        private IFS fs = null;
        bool stopped = false;

        private enum HandleStatus
        {
            NEW = 0,
            IN_PROGRESS,
            COMPLETED
        }
        private HandleStatus[] statuses;

        public Traversal(Graph graph)
        {
            this.graph = graph;
            fs = new T();
            fs.Init(graph);
            statuses = new HandleStatus[graph.GetVerticesCount()];
        }

        //events, which trigger when vertex is handled with correspond HandleStatus
        public EventHandler<int> NewVertex;
        public EventHandler<int> VertexInProgress;
        public EventHandler<int> CompletedVertex;

        public void Run(int vertex = 0)
        {
            stopped = false;

            for (int i = 0; i < statuses.Count(); ++i)
            {
                statuses[i] = HandleStatus.NEW;
            }
            HandleVertexNotify(vertex);

            var container = fs.GetContainer();

            container.Clear();
            container.Add(vertex);

            statuses[vertex] = HandleStatus.IN_PROGRESS;

            while (container.Count() != 0)
            {
                var v = container.Peek();
                container.Remove();

                int[] vertices;
                graph.GetAdjVertices(v, out vertices);
                foreach (var el in vertices)
                {
                    HandleVertexNotify(el);

                    if (stopped) return;

                    if (statuses[el] == HandleStatus.NEW)
                    {
                        statuses[el] = HandleStatus.IN_PROGRESS;
                        container.Add(el);
                    }
                }
                statuses[v] = HandleStatus.COMPLETED;
            }
        }

        public void Stop()
        {
            stopped = true;
        }

        private void HandleVertexNotify(int vertex)
        {
            switch (statuses[vertex])
            {
                case HandleStatus.NEW:
                    NewVertex?.Invoke(this, vertex);
                    break;
                case HandleStatus.IN_PROGRESS:
                    VertexInProgress?.Invoke(this, vertex);
                    break;
                case HandleStatus.COMPLETED:
                    CompletedVertex?.Invoke(this, vertex);
                    break;
            }
        }
    }

    public class Traversal : Traversal<DFS>
    {
        public Traversal(Graph graph) : base(graph) { }
    }
}
