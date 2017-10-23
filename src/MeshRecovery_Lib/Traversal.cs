using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeshRecovery_Lib
{

    public interface IFS
    {
        IContainer GetContainer();
    }

    public class DFS: IFS
    {
        private DFSContainer container;

        public DFS()
        {
            container = new DFSContainer();
        }

        public IContainer GetContainer()
        {
            return container;
        }
    }

    public class BFS : IFS
    {
        private BFSContainer container;

        public BFS()
        {
            container = new BFSContainer();
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
            statuses = new HandleStatus[graph.GetVerticesCount()];
        }

        //events, which trigger when vertex is handled with correspond HandleStatus
        public EventHandler<int> NewVertex;
        public EventHandler<int> VertexInProgress;
        public EventHandler<int> CompletedVertex;

        public void Run(int vertex = 0)
        {
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
                var v = container.Get();
                container.Remove();

                int[] vertices;
                graph.GetAdjVertices(v, out vertices);
                foreach (var el in vertices)
                {
                    HandleVertexNotify(el);
                    if (statuses[el] == HandleStatus.NEW)
                    {
                        statuses[el] = HandleStatus.IN_PROGRESS;
                        container.Add(el);
                    }
                }
                statuses[v] = HandleStatus.COMPLETED;
            }
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
