using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeshRecovery_Lib
{
    public class Traversal
    {
        private Graph graph;

        private enum HandleStatus
        {
            NEW = 0,
            IN_PROGRESS,
            COMPLETED
        }
        private HandleStatus[] statuses;

        public enum Direction
        {
            DFS = 0,
            BFS,
            LAST
        }
        public Direction DIRECTION { set; get; }

        private MethodInfo[] algorithmFuncs;

        public Traversal(Graph graph)
        {
            this.graph = graph;
            DIRECTION = Direction.DFS;
            statuses = new HandleStatus[graph.GetVerticesCount()];
            algorithmFuncs = new MethodInfo[Convert.ToInt32(Direction.LAST)];// { DFS, BFS };
            for (Direction d = Direction.DFS; d < Direction.LAST; ++d)
            {
                var name = Enum.GetName(typeof(Direction), d);
                algorithmFuncs[Convert.ToInt32(d)] = this.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
            }
        }

        //events, which trigger when vertex is handled with correspond HandleStatus
        public EventHandler<int> NewVertex;
        public EventHandler<int> VertexInProgress;
        public EventHandler<int> CompletedVertex;

        public void Run()
        {
            for (int i = 0; i < statuses.Count(); ++i)
            {
                statuses[i] = HandleStatus.NEW;
            }
            HandleVertexNotify(0);
            algorithmFuncs[Convert.ToInt32(DIRECTION)].Invoke(this, new object[] { 0 });
        }

        private void DFS(int vertex)
        {
            statuses[vertex] = HandleStatus.IN_PROGRESS;

            int[] vertices;
            graph.GetAdjVertices(vertex, out vertices);
            foreach (var v in vertices)
            {
                HandleVertexNotify(v);
                if (statuses[v] == HandleStatus.NEW)
                {
                    DFS(v);
                }
            }
            statuses[vertex] = HandleStatus.COMPLETED;
        }

        private void BFS(int vetrex)
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(vetrex);

            statuses[vetrex] = HandleStatus.IN_PROGRESS;

            while (queue.Count() != 0)
            {
                var v = queue.Dequeue();
                int[] vertices;
                graph.GetAdjVertices(v, out vertices);
                foreach (var el in vertices)
                {
                    HandleVertexNotify(el);
                    if (statuses[el] == HandleStatus.NEW)
                    {
                        statuses[el] = HandleStatus.IN_PROGRESS;
                        queue.Enqueue(el);
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
}
