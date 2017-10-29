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

            meshDimension = GetDimension(graph);

            if (meshDimension == 0)
                return false;

            return true;
        }
        /// <summary>
        /// Method retuns the dimension of the graph and checks if graph is conected
        /// </summary>
        /// <param name="graph">It`s a graph</param>
        /// <returns>
        /// Return code:
        /// 0 - error
        /// 1 - ono dimension
        /// 2 - two dimension
        /// 3 - three dimension 
        /// </returns>
        private static int GetDimension(Graph graph)
        {
            int V = graph.GetVerticesCount();
            int E = graph.GetEdgeCount();
            long max_adj = 0;

            Traversal t = new Traversal(graph);

            List<long> vertices = new List<long>();
            t.NewVertex += (sender, e) =>
            {
                if (max_adj < graph.GetAdjVerticesCount(e))
                    max_adj = graph.GetAdjVerticesCount(e);
                vertices.Add(e);
            };
            t.Run();

            if (vertices.Count() != graph.GetVerticesCount())
                return 0;

            if (V - E + 1 == 2) // should be one dimension graph ( chain )
            {

                return 1;
            }

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
                case 2: return TwoDimensionNumerate(graph, out graphNumeration); 
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

        /// <summary>
        /// Method restores geometry information for graph in 2 dimension
        /// </summary>
        /// <param name="graph">It`s a graph, Mr. Graph</param>
        /// <param name="graphNumeration">Restored numeration for the input graph</param>
        /// <returns>
        /// Return code:
        /// 0 - success
        /// -1 - error
        private static int TwoDimensionNumerate(Graph graph, out int[][] graphNumeration)
        {
            
            Traversal traversal = new Traversal(graph);
            int V = graph.GetVerticesCount();
            int[][] res_graphNumeration = new int[V][];
            int[][] history_guess = new int[V][]; // which vertex and which index was increased and where
            int index_guess = 0;
            graphNumeration = res_graphNumeration;
            int start_vertex = -1;
            int another_start_vertex = -1;

            //find start vertex 
            traversal.NewVertex = (sender, e) =>
            {
                if ((graph.GetAdjVerticesCount(e) == 3))
                    another_start_vertex = e;
                if ((graph.GetAdjVerticesCount(e) == 4))
                    start_vertex = e;  
            };
            traversal.Run();
            if (another_start_vertex == -1 && start_vertex == -1)
                return -1; // not two dimension graph
            else if (start_vertex == -1)
                start_vertex = another_start_vertex;
            bool fixed_first = false; //
            //set start information
            res_graphNumeration[start_vertex] =new int[] {0,0};
            int[] buf;
            long count_around_start = graph.GetAdjVertices(start_vertex,  out buf);
            int x_start=1, y_start=0; 
            for (int i = 0; i < count_around_start; ++i)
            {
                res_graphNumeration[buf[i]] = new int[] { x_start, y_start }; // NB: make it easy
                y_start = x_start;
                if (--x_start == -2) x_start = 0;
            }
            traversal.NewVertex = (sender, e) =>
            {
                if (res_graphNumeration[e]==null)
                {
                    int[] buff;
                    long adj_count = graph.GetAdjVertices(e, out buff);
                    //try to determine with al least 2 numerated vertex
                    int v1=-1, v2=-1;
                    for (int i = 0; i<adj_count;++i)
                    {
                        if (res_graphNumeration[buff[i]] != null)
                        {
                            v2 = v1;
                            v1 = buff[i];
                        }
                    }
                    if (v1 == -1 || v2 == -1)
                    {//if not
                     //check that there is vertex with all pohers numerated neighbours 
                        if (v1!= -1)
                        {
                            adj_count = graph.GetAdjVertices(v1, out buff);
                            int numerated = 0;
                            int x = 0, y = 0;
                            for (int i = 0; i < adj_count; ++i)
                            {
                                if (res_graphNumeration[buff[i]] != null)
                                {
                                    x+= res_graphNumeration[buff[i]][0];
                                    y+= res_graphNumeration[buff[i]][1];
                                    ++numerated;
                                }
                            }
                            if (adj_count - numerated == 1)
                            {
                                if (adj_count==4)
                                {
                                    res_graphNumeration[e] = new int[] { res_graphNumeration[v1][0]*4-x , res_graphNumeration[v1][1] * 4 - y };
                                }
                                else
                                {
                                    int x_direction = (res_graphNumeration[v1][0] * 4 - 1>0)?1:-1;
                                    int y_direction = (res_graphNumeration[v1][1] * 4 - 1 > 0) ? 1 : -1;
                                    res_graphNumeration[e] = new int[] { res_graphNumeration[v1][0] + x_direction, res_graphNumeration[v1][1]  };
                                    history_guess[index_guess++] = new int[] { e, x_direction, y_direction};
                                }
                                
                            }
                            //here we give up 
                        }
                            
                    }
                    else
                    {
                        //special case of start vertex
                        //swap v1 and reversed from start vertex neighbours
                        if ((res_graphNumeration[v1][0] == res_graphNumeration[v2][0])|| (res_graphNumeration[v1][1] == res_graphNumeration[v2][1]))
                        {
                            if (!fixed_first)
                            {
                                for (int i = 0; i < count_around_start; ++i)
                                {
                                    if ((res_graphNumeration[buf[i]][0] == res_graphNumeration[v1][1]) && (res_graphNumeration[buf[i]][1] == res_graphNumeration[v1][0]))
                                    {
                                        int[] temp = res_graphNumeration[v1];
                                        res_graphNumeration[v1] = res_graphNumeration[buf[i]];
                                        res_graphNumeration[buf[i]] = temp;
                                        fixed_first = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                res_graphNumeration[history_guess[index_guess - 1][0]] = new int[] { res_graphNumeration[history_guess[index_guess - 1][0]][0] - history_guess[index_guess - 1][1], res_graphNumeration[history_guess[index_guess - 1][0]][1] + history_guess[index_guess - 1][2] };
                            }
                        }

                        int x = Math.Max( Math.Abs(res_graphNumeration[v1][0]), Math.Abs(res_graphNumeration[v2][0]));
                        int y = Math.Max(Math.Abs(res_graphNumeration[v1][1]), Math.Abs(res_graphNumeration[v2][1]));
                        if (res_graphNumeration[v1][0] + res_graphNumeration[v2][0] < 0) x = x * -1;
                        if (res_graphNumeration[v1][1] + res_graphNumeration[v2][1] < 0) y = y * -1;
                        res_graphNumeration[e] = new int[] { x, y };
                    }
                    //here we give up
                    //but we will do it again!!
                }
            };
            
            traversal.Run(start_vertex);
            graphNumeration = res_graphNumeration;
            return 0;
        }
    }
}
