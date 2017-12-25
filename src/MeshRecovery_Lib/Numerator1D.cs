using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    class Numerator1D: INumerator
    {
        /// <summary>
        /// Method restores geometry information for graph
        /// </summary>
        /// <param name="graph">It`s a graph, Mr. Graph</param>
        /// <param name="graphNumeration">Restored numeration for the input graph with one dimension</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        /// </returns>
        public int Run(Graph graph, out int[][] graphNumeration)
        {
            Traversal traversal = new Traversal(graph);

            int V = graph.GetVerticesCount();
            int index = V;
            int start = -1;
            graphNumeration = new int[V][];
            int[][] res_graphNumeration = new int[V][];
            traversal.NewVertex = (sender, e) =>
            {
                if (graph.GetAdjVerticesCount(e) == 1)
                    start = e;
            };
            traversal.Run();

            if (start == -1)
                return -1;

            traversal.NewVertex = (sender, e) =>
            {
                res_graphNumeration[e] = new int[] { V-- };
            };
            traversal.Run(start);
            graphNumeration = res_graphNumeration;
            if (V != 0)
                return -1;
            else
                return 0;
        }
    }
}
