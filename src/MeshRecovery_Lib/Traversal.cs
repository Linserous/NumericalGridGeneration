using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeshRecovery_Lib
{
    public interface IFS
    {
        void insert(ref List<int> list, int element);
    }

    public class DFS : IFS
    {
        public void insert(ref List<int> list, int element)
        {
            list.Insert(0, element);
        }
    }

    public class BFS : IFS
    {
        public void insert(ref List<int> list, int element)
        {
            list.Add(element);
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

            List<int> list = new List<int>() { vertex };

            statuses[vertex] = HandleStatus.IN_PROGRESS;

            while (list.Count() != 0)
            {
                var v = list.First();
                list.RemoveAt(0);
                int[] vertices;
                graph.GetAdjVertices(v, out vertices);
                foreach (var el in vertices)
                {
                    HandleVertexNotify(el);
                    if (statuses[el] == HandleStatus.NEW)
                    {
                        statuses[el] = HandleStatus.IN_PROGRESS;
                        fs.insert(ref list, el);
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
