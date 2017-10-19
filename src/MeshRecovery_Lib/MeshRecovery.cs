﻿using System;
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

            if (IsConnected(graph))
            {
                switch (meshDimension = GetDimension(graph))
                {
                    case 1: return OneDimensionValidate(graph);
                    //TODO: implement this part
                    case 2: return true;
                    case 3: return true;
                    default: return false;
                }
            }

            return false;
        }
        /// <summary>
        /// Methon checks if graph is connected
        /// </summary>
        /// <param name="graph">It's a graph</param>
        /// <returns>
        /// true - graph is connected
        /// false - graph is not connected
        /// </returns>
        private static bool IsConnected(Graph graph)
        {
            Traversal t = new Traversal(graph);

            List<int> vertices = new List<int>();
            t.NewVertex += (sender, e) => vertices.Add(e);
            t.Run();

            if (vertices.Count() == graph.GetVerticesCount())
                return true;
            else
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
            long max_adj = 0;

            if (V - E + 1 == 2) // should be one dimension graph ( chain )
            {

                return 1;
            }

            for (int i = 0; i < V; i++)
                if (max_adj < graph.GetAdjVerticesCount(i))
                    max_adj = graph.GetAdjVerticesCount(i);

            if (max_adj <= 4)
            {
                return 2;
            }

            if (max_adj <= 6)
            {
                return 3;
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

            Traversal traversal = new Traversal(graph);

            int V = graph.GetVerticesCount();
            bool[] union = new bool[V]; 

            for (int i = 0; i < V; ++i)
            {
                union[i] = false;
            }

            traversal.NewVertex += (sender, e) =>
            {
                union[e] = true;
            };
            traversal.Run();

            for (int i = 0; i < V; ++i)
            {
                if (union[i] == false) return false;
            }
            return true;    
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
            Traversal traversal = new Traversal(graph);

            int V = graph.GetVerticesCount();
            int[] found = new int[V];
            bool start = false;
            int found_index = 0;
            graphNumeration = new int[V][];
            traversal.NewVertex += (sender, e) =>
            {
                if ((graph.GetAdjVerticesCount(e)==1)&&(!start))
                {
                    start = true;
                    found[found_index] = e ;
                    for (int i = 0; i < found_index/2; ++i)
                    {
                        int tmp = found[i];
                        found[i] = found[found_index - i];
                        found[found_index - i ] = tmp;
                    }
                    ++found_index;
                }
                else
                    found[found_index++] = e;
            };
            traversal.Run();
            for (int i = 0; i < V; ++i)
            {
                graphNumeration[found[i]] = new int[] { i + 1 };
            }

            if (found_index != V) 
                return -1;
            else
                return 0;
        }

    }
}
