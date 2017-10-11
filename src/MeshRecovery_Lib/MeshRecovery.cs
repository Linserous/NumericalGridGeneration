using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeshRecovery_Lib
{
    
    public static class MeshRecovery
    {
        /// <summary>
        /// Method checks that input graph corresponds to a regular grid
        /// </summary>
        /// <param name="xadj">Index array for graph</param>
        /// <param name="size">Size of index array</param>
        /// <param name="adjncy">Adjacency list</param>
        /// <param name="meshDimension">Dimension of a regular grid that corresponds to the input graph</param>
        /// <returns>true if graph corresponds to a regular grid, otherwise false</returns>
        public static bool Validate(long[] xadj, int size, int[] adjncy, out int meshDimension)
        {
            // TODO: Implement for 2,3 dimensions
            meshDimension = 0;

            Graph graph = new Graph(xadj, adjncy);
            // fast check of valid 
            if (graph.GetVerticesCount() != size - 1)
            {
                return false;
            }

            switch (GetDimension(graph))
            {
                case 1: return OneDimensionValidate(graph); 
                //TODO: implement this part
                case 2: break;
                case 3: break;
                default: return false;
            }
              
            return false;
        }
        /// <summary>
        /// Method retuns the dimension of the graph
        /// </summary>
        /// <param name="graph">It`s a graph</param>
        /// <returns>
        /// Return code:
        /// 0 - error
        /// 1 - ono dimension
        /// 2 - two dimension
        /// 3 - three dimension 
        /// </returns>
        private static int GetDimension(Graph  graph)
        {
            int V = graph.GetVerticesCount();
            int E = graph.GetEdgeCount();
            
            if (V - E + 1 == 2) // should be one dimension graph ( chain )
            {

                return 1;
            }
            else
            {
                //TODO: this is not a chain... 
            }

            return 0;

        }
        /// <summary>
        /// Method restores geometry information for each graph node
        /// </summary>
        /// <param name="xadj">Index array for graph</param>
        /// <param name="size">Size of index array</param>
        /// <param name="adjncy">Adjacency list</param>
        /// <param name="graphNumeration">Restored numeration for the input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        /// </returns>
        public static int Numerate(long[] xadj, int size, int[] adjncy, out int[][] graphNumeration)
        {
            // TODO: Implement
            // TODO: graphNumeration may be 1-3 dimensional array
             
            Graph graph = new Graph(xadj, adjncy);

            graphNumeration = null;
            int V = graph.GetVerticesCount();

            switch (GetDimension(graph))
            {
                case 1: return OneDimensionNumerate(graph, out graphNumeration);
                //TODO: implement this part
                case 2: break;
                case 3: break;
                default: return -1;
            }

            return -1;
        }

        /// <summary>
        /// Method validate structure of one dimension graph
        /// </summary>
        /// <param name="graph">It`s a graph</param>
        /// <returns>
        /// Return code:
        /// true - correct structure
        /// false - not correct structure
        /// </returns>
        private static bool OneDimensionValidate(Graph graph)
        {
            //I was sure that one dimension graphs are good boys, but then..
            // 0--0
            //  \/   0--0
            //  0
            // 
            int V = graph.GetVerticesCount();
            int[] union = new int[V]; // minor TODO: change to bool
            int i;
            int start=-1;
            for ( i = 0; i < V; ++i)
            {
                if (graph.GetAdjVerticesCount(i) == 1) start = i;
                union[i] = -1;
            }
            i = 0;
            int prev_vertex=start;
            union[start] = start;
            int[] buff;
            graph.GetAdjVertices(prev_vertex, out buff);
            int next_vertex = buff[0]; 
            while(true)
            {
                if (graph.GetAdjVerticesCount(next_vertex) == 1)
                {
                    union[next_vertex] = next_vertex;
                    for (int j = 0; j <union.Length; ++j)
                        if (union[j] == -1)
                        {
                            return false;
                        }
                    return true;
                }

                union[next_vertex] = next_vertex;
                long adjcount = graph.GetAdjVertices(next_vertex, out buff);

                int temp = next_vertex;
                for (int j = 0; j < adjcount; ++j)
                    if (buff[j] != prev_vertex) next_vertex = buff[j];
                
                if (union[next_vertex] != -1) return false;
                prev_vertex = temp;
            }            
        }
        /// <summary>
        /// Method restores geometry information for graph
        /// </summary>
        /// <param name="graph">It`s a graph, Mr. Graph</param>
        /// <param name="graphNumeration">Restored numeration for the input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        /// </returns>
        private static int OneDimensionNumerate(Graph graph, out int[][] graphNumeration)
        { 
            int V = graph.GetVerticesCount();
            int origV = V;
            graphNumeration = new int[V][];
            int i;
            int start = -1;
            for (i = 0; i < V; ++i)
            {
                if (graph.GetAdjVerticesCount(i) == 1)
                {
                    start = i;
                    break;
                }
            }
            i = 1;
            int prev_vertex = start;
            graphNumeration[start] = new int[] { i };
            int[] buff;
            graph.GetAdjVertices(prev_vertex, out buff);
            int next_vertex = buff[0];
            while (V>0)
            {
                
                if (graph.GetAdjVerticesCount(next_vertex) == 1)
                {
                    graphNumeration[next_vertex] = new int[] { ++i };
                    for (int j = 0; j < graphNumeration.Length; ++j)
                        if (graphNumeration[j]==null)
                        {
                            graphNumeration = null;
                            return -1;
                        }
                    return 0;                    
                }

                graphNumeration[next_vertex] = new int[] { ++i };
                long adjcount = graph.GetAdjVertices(next_vertex, out buff);

                for (int j = 0; j < adjcount; ++j)
                    if (buff[j] != prev_vertex) next_vertex = buff[j];
                --V;
            }
            return -1;
        }

    }
}
