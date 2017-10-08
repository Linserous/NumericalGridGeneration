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
    
        private enum Color
        {
            WHITE = 0,
            GRAY,
            BLACK
        }
        private Color[] vColors;

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
            vColors = new Color[graph.GetVerticesCount()];
        }
        
        //delegate, which triggers when new vertex was handled
        public delegate void TraverseCallback(int vertex);

        public void run(TraverseCallback call)
        {
            for(int i = 0; i < vColors.Count(); ++i)
            {
                vColors[i] = Color.WHITE;
            }
            switch(DIRECTION)
            {
                case Direction.DFS:
                    dfs(0, call);
                    break;
                case Direction.BFS:
                    bfs(0, call);
                    break;
            }
        }

        private void dfs(int vertex, TraverseCallback call)
        {
            call(vertex);
            vColors[vertex] = Color.GRAY;
            int[] vertices;
            graph.GetAdjVertices(vertex, out vertices);
            foreach (var v in vertices)
            {
                if (vColors[v] == Color.WHITE)
                {
                    dfs(v, call);
                }
            }
            vColors[vertex] = Color.BLACK;
        }

        private void bfs(int vertex, TraverseCallback call)
        {
            Queue<int> queue = new Queue<int>();
            call(vertex);
            queue.Enqueue(vertex);
            vColors[vertex] = Color.GRAY;

            while (queue.Count() != 0)
            {
                var v = queue.Dequeue();
                int[] vertices;
                graph.GetAdjVertices(v, out vertices);
                foreach (var el in vertices)
                {
                    if (vColors[el] == Color.WHITE)
                    {
                        call(el);
                        vColors[el] = Color.GRAY;
                        queue.Enqueue(el);
                    }
                }
                vColors[v] = Color.BLACK;
            }
        }
    }
}
