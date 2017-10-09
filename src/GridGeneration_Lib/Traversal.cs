using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            BFS
        }
        public Direction DIRECTION { set; get; }

        public Traversal(Graph graph)
        {
            this.graph = graph;
            DIRECTION = Direction.DFS;
            statuses = new HandleStatus[graph.GetVerticesCount()];
        }

        //events, which trigger when vertex is handled with correspond HandleStatus
        public EventHandler<int> NewVertex;
        public EventHandler<int> VertexInProgress;
        public EventHandler<int> CompletedVertex;

        public void Run()
        {
            for(int i = 0; i < statuses.Count(); ++i)
            {
                statuses[i] = HandleStatus.NEW;
            }
            HandleVertexNotify(0);

            switch (DIRECTION)
            {
                case Direction.DFS:
                    DFS();
                    break;
                case Direction.BFS:
                    BFS();
                    break;
            }
        }

        private void DFS(int vertex = 0)
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

        private void BFS()
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(0);

            statuses[0] = HandleStatus.IN_PROGRESS;

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
                   if (NewVertex != null) NewVertex(this, vertex);
                    break;
                case HandleStatus.IN_PROGRESS:
                    if (VertexInProgress != null) VertexInProgress(this, vertex);
                    break;
                case HandleStatus.COMPLETED:
                    if (CompletedVertex != null) CompletedVertex(this, vertex);
                    break;
            }
        }
    }
}
